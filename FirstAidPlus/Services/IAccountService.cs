using FirstAidPlus.Models;

namespace FirstAidPlus.Services
{
    public interface IAccountService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> RegisterUserAsync(FirstAidPlus.ViewModels.RegisterViewModel model);
        Task<User> GetOrCreateGoogleUserAsync(string email, string fullName, string googleId);
        Task<ViewModels.EditProfileViewModel?> GetProfileAsync(string username);
        Task<bool> UpdateProfileAsync(string username, ViewModels.EditProfileViewModel model);
        Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        Task<bool> ConfirmEmailAsync(string email, string token);
    }
}
