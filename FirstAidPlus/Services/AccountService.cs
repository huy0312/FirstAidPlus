using FirstAidPlus.Models;
using FirstAidPlus.Repositories;

namespace FirstAidPlus.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;

        public AccountService(IUserRepository userRepository, Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
        {
            _userRepository = userRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }



            // Verify hashed password
            bool verified = false;
            try
            {
               // Handle legacy plain-text passwords (if any exist from seeding) or BCrypt hash
               if (user.PasswordHash == password) verified = true; // Fallback for plain text
               else if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) verified = true;
            }
            catch 
            {
                // In case of invalid hash format
                return null; 
            }

            if (verified)
            {
                return user;
            }

            return null;
        }

        public async Task<User?> RegisterUserAsync(ViewModels.RegisterViewModel model)
        {
          
            var existingUser = await _userRepository.GetUserByUsernameAsync(model.Username);
            if (existingUser != null) return null;

            var existingEmail = await _userRepository.GetUserByEmailAsync(model.Email);
            if (existingEmail != null) return null;

            
            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = 3,
                IsEmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };

            return await _userRepository.CreateUserAsync(newUser);
        }

        public async Task<User> GetOrCreateGoogleUserAsync(string email, string fullName, string googleId)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                return user;
            }

            // Create new user from Google
            var newUser = new User
            {
                Username = email.Split('@')[0], // Simple username generation
                Email = email,
                FullName = fullName,
                PasswordHash = "GOOGLE_AUTH_" + googleId, // Placeholder
                RoleId = 3, // User Role
                IsEmailConfirmed = true // Google emails are verified
            };
            
            // Handle duplicate username edge case in production (e.g. append random number)
            var existingUser = await _userRepository.GetUserByUsernameAsync(newUser.Username);
            if (existingUser != null)
            {
                newUser.Username = $"{newUser.Username}_{new Random().Next(1000, 9999)}";
            }

            return await _userRepository.CreateUserAsync(newUser);
        }

        public async Task<ViewModels.EditProfileViewModel?> GetProfileAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return null;

            return new ViewModels.EditProfileViewModel
            {
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Bio = user.Bio,
                ExistingAvatarUrl = user.AvatarUrl
            };
        }

        public async Task<bool> UpdateProfileAsync(string username, ViewModels.EditProfileViewModel model)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return false;

            if (model.AvatarFile != null)
            {
                // Basic validation (can be improved)
                if (model.AvatarFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                    Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AvatarFile.CopyToAsync(fileStream);
                    }

                    user.AvatarUrl = "/images/avatars/" + uniqueFileName;
                }
            }

            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.Address = model.Address;
            user.Bio = model.Bio;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null) return false;

            // Verify current
            bool verified = false;
            if (user.PasswordHash == currentPassword) verified = true;
            else if (BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash)) verified = true;

            if (!verified) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return null;

            // Generate simple token (for prod use proper secure token generator)
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _userRepository.UpdateUserAsync(user);
            return token;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return false;

            if (user.ResetToken != token) return false;
            if (user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _userRepository.UpdateUserAsync(user);
            return true;
        }
        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
             var user = await _userRepository.GetUserByEmailAsync(email);
             if (user == null) return false;
             
             // In production, also check expiry if needed, or if already confirmed
             if (user.IsEmailConfirmed) return true; 

             if (user.EmailConfirmationToken != token) return false;

             user.IsEmailConfirmed = true;
             user.EmailConfirmationToken = null;
             await _userRepository.UpdateUserAsync(user);
             return true;
        }
    }
}
