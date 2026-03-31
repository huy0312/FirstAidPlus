using FirstAidPlus.Data;
using FirstAidPlus.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FirstAidPlus.Controllers
{
    public class AccountController : Controller
    {
        private readonly FirstAidPlus.Services.IAccountService _accountService;
        private readonly FirstAidPlus.Services.IEmailService _emailService;
        private readonly FirstAidPlus.Services.IAIService _aiService;
        private readonly AppDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<FirstAidPlus.Hubs.NotificationHub> _hubContext;

        public AccountController(
            FirstAidPlus.Services.IAccountService accountService, 
            FirstAidPlus.Services.IEmailService emailService, 
            FirstAidPlus.Services.IAIService aiService,
            AppDbContext context,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment,
            Microsoft.AspNetCore.SignalR.IHubContext<FirstAidPlus.Hubs.NotificationHub> hubContext)
        {
            _accountService = accountService;
            _emailService = emailService;
            _aiService = aiService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // If accessed directly, redirect to Home. Modal should be used instead.
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["ShowLoginModal"] = true;
                TempData["ReturnUrl"] = returnUrl;
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Vui lòng nhập tên đăng nhập và mật khẩu." });
            }

            var user = await _accountService.ValidateUserAsync(username, password);

            if (user == null)
            {
                return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng." });
            }

            if (!user.IsEmailConfirmed)
            {
                return Json(new { success = false, message = "Vui lòng xác thực tài khoản qua email trước khi đăng nhập." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("FullName", user.FullName ?? ""),
                new Claim("AvatarUrl", user.AvatarUrl ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            string redirectUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : Url.Action("Index", "Home");
            
            if (user.Role?.RoleName == "Admin" && string.IsNullOrEmpty(returnUrl))
            {
                redirectUrl = Url.Action("Index", "Dashboard", new { area = "Admin" });
            }

            return Json(new { success = true, redirectUrl });
        }


        [HttpGet]
        public IActionResult Register()
        {
            // Registration is now handled via modal, redirect to home
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(ViewModels.RegisterViewModel model, string returnUrl = null)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = string.Join(". ", errors) });
                }

                // AI Certificate Verification for Experts
                if (model.IsExpert)
                {
                    if (model.CertificateFile == null || model.CertificateFile.Length == 0)
                    {
                        return Json(new { success = false, message = "Vui lòng tải lên ảnh chứng chỉ chuyên môn để xác thực." });
                    }

                    var (isValid, aiMessage) = await _aiService.VerifyCertificateAsync(model.CertificateFile, model.FullName, model.ExperienceYears);
                    if (!isValid)
                    {
                        return Json(new { success = false, message = $"Xác thực chứng chỉ không hợp lệ: {aiMessage}" });
                    }
                }

                var user = await _accountService.RegisterUserAsync(model);
                if (user == null)
                {
                    return Json(new { success = false, message = "Email này đã được sử dụng. Vui lòng dùng email khác." });
                }

                // Handle Expert Registration
                if (model.IsExpert)
                {
                    // Update user bio with expert info if provided
                    var expertInfo = $"Chức danh: {model.ExpertTitle} - Nơi làm làm việc: {model.Workplace} - {model.ExperienceYears} năm kinh nghiệm";
                    user.Bio = string.IsNullOrEmpty(user.Bio) ? expertInfo : user.Bio + " | " + expertInfo;
                    
                    // Handle Certificate File Upload
                    if (model.CertificateFile != null && model.CertificateFile.Length > 0)
                    {
                        var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "certificates");
                        Directory.CreateDirectory(uploadFolder);
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CertificateFile.FileName;
                        var filePath = Path.Combine(uploadFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.CertificateFile.CopyToAsync(fileStream);
                        }
                        
                        var qualification = new Qualification
                        {
                            UserId = user.Id,
                            Title = "Chứng chỉ chuyên môn",
                            Description = expertInfo,
                            CertificateUrl = "/images/certificates/" + uniqueFileName,
                            IssuedAt = DateTime.UtcNow,
                            Status = Qualification.StatusPending
                        };
                        _context.Qualifications.Add(qualification);
                    }

                    // Set RoleId=2 (Expert) immediately
                    user.RoleId = 2;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    // Send Email to User
                    var emailSender = HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender>();
                    if (emailSender != null)
                    {
                        string emailSubject = "Đơn đăng ký chuyên gia đang chờ phê duyệt - FirstAidPlus";
                        string emailBody = $@"
                        <h3>Xin chào {user.FullName},</h3>
                        <p>Cảm ơn bạn đã đăng ký trở thành chuyên gia trên FirstAidPlus.</p>
                        <p>Đơn đăng ký của bạn hiện đang ở trạng thái <strong>Chờ phê duyệt</strong> bởi Ban quản trị.</p>
                        <p>Chúng tôi sẽ liên hệ với bạn để xác minh thông tin trước khi cấp quyền Chuyên gia.</p>
                        <br/>
                        <p>Trân trọng,</p>
                        <p>Đội ngũ FirstAidPlus</p>";
                        
                        Task.Run(() => emailSender.SendEmailAsync(user.Email, emailSubject, emailBody));
                    }
                }

                // Send Confirmation Email in background (fire-and-forget) to avoid slow SMTP blocking the response
                if (!string.IsNullOrEmpty(user.EmailConfirmationToken))
                {
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token = user.EmailConfirmationToken, email = user.Email }, Request.Scheme);
                    var userEmail = user.Email;
                    var userFullName = user.FullName ?? user.Username;
                    var emailService = _emailService;

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var message = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                                    <h1 style='color: white; margin: 0;'>Chào mừng đến với FirstAid+</h1>
                                </div>
                                <div style='background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none;'>
                                    <p style='font-size: 16px; color: #333;'>Xin chào <strong>{userFullName}</strong>,</p>
                                    <p style='color: #666; line-height: 1.6;'>Cảm ơn bạn đã đăng ký tài khoản tại <strong>FirstAid+</strong>. Để hoàn tất quá trình đăng ký và bắt đầu trải nghiệm các khóa học sơ cấp cứu chuyên nghiệp, vui lòng xác thực email của bạn.</p>
                                    <div style='text-align: center; margin: 30px 0;'>
                                        <a href='{confirmationLink}' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 14px 40px; text-decoration: none; border-radius: 25px; font-weight: bold; display: inline-block;'>Xác Thực Tài Khoản</a>
                                    </div>
                                    <p style='color: #999; font-size: 13px;'>Nếu nút không hoạt động, bạn có thể sao chép đường link sau vào trình duyệt:</p>
                                    <p style='color: #667eea; font-size: 12px; word-break: break-all;'>{confirmationLink}</p>
                                    <hr style='border: none; border-top: 1px solid #eee; margin: 25px 0;'/>
                                    <p style='color: #999; font-size: 12px;'>Link này sẽ hết hạn sau 24 giờ. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>
                                </div>
                                <div style='background: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px;'>
                                    <p style='margin: 0; color: #666; font-size: 12px;'>© 2024 FirstAid+. Mọi quyền được bảo lưu.</p>
                                </div>
                            </div>";
                            await emailService.SendEmailAsync(userEmail, "[FirstAid+] Xác thực tài khoản của bạn", message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Email error: {ex.Message}");
                        }
                    });
                }

                if (model.IsExpert)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("RegisterSuccess", "Account", new { expert = true }) });
                }
                return Json(new { success = true, redirectUrl = Url.Action("RegisterSuccess", "Account") });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}\n{ex.StackTrace}");
                return Json(new { success = false, message = "Đã có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại sau." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Token hoặc Email không hợp lệ.";
                return View("EmailConfirmed"); // Or Error view
            }

            var result = await _accountService.ConfirmEmailAsync(email, token);
            if (result)
            {
                ViewBag.Success = "Xác thực email thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return View("EmailConfirmed");
            }
            else
            {
                ViewBag.Error = "Xác thực thất bại. Token có thể đã hết hạn.";
                return View("EmailConfirmed");
            }
        }

        [HttpGet]
        public IActionResult RegisterSuccess(bool expert = false)
        {
            ViewBag.IsExpert = expert;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var googleId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = await _accountService.GetOrCreateGoogleUserAsync(email, name ?? email, googleId ?? "unknown");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("FullName", user.FullName ?? ""),
                new Claim("AvatarUrl", user.AvatarUrl ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var username = User.Identity.Name;
            var profile = await _accountService.GetProfileAsync(username);
            
            if (profile == null)
            {
                return RedirectToAction("Login");
            }
            
            return View(profile);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> EditProfile(ViewModels.EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var username = User.Identity.Name;
            var result = await _accountService.UpdateProfileAsync(username, model);

            if (!result)
            {
                ViewBag.Error = "Cập nhật thất bại. Vui lòng thử lại.";
                return View(model);
            }

            // Refresh cookie claims to show new Avatar/Name immediately
            var updatedProfile = await _accountService.GetProfileAsync(username);
            if (updatedProfile != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, updatedProfile.Username ?? username),
                    new Claim(ClaimTypes.NameIdentifier, User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""),
                    new Claim(ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role) ?? "User"),
                    new Claim("FullName", updatedProfile.FullName ?? ""),
                    new Claim("AvatarUrl", updatedProfile.ExistingAvatarUrl ?? "")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            // Update model with potential new data (like AvatarUrl)
            if (updatedProfile != null)
            {
                model.ExistingAvatarUrl = updatedProfile.ExistingAvatarUrl;
            }
            return View(model);
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword(ViewModels.ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var username = User.Identity.Name;
            var result = await _accountService.ChangePasswordAsync(username, model.CurrentPassword, model.NewPassword);

            if (!result)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng.";
                return View(model);
            }

            ViewBag.Success = "Đổi mật khẩu thành công!";
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ViewModels.ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _accountService.GeneratePasswordResetTokenAsync(model.Email);
            if (token != null)
            {
                var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);
                var message = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                            <h1 style='color: white; margin: 0;'>Đặt Lại Mật Khẩu</h1>
                        </div>
                        <div style='background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none;'>
                            <p style='font-size: 16px; color: #333;'>Xin chào,</p>
                            <p style='color: #666; line-height: 1.6;'>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản <strong>FirstAid+</strong> của bạn. Nhấp vào nút bên dưới để tạo mật khẩu mới:</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{resetLink}' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 14px 40px; text-decoration: none; border-radius: 25px; font-weight: bold; display: inline-block;'>Đặt Lại Mật Khẩu</a>
                            </div>
                            <p style='color: #999; font-size: 13px;'>Nếu nút không hoạt động, sao chép đường link sau vào trình duyệt:</p>
                            <p style='color: #667eea; font-size: 12px; word-break: break-all;'>{resetLink}</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 25px 0;'/>
                            <div style='background: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 8px;'>
                                <p style='margin: 0; color: #856404; font-size: 13px;'><strong>⚠️ Lưu ý bảo mật:</strong> Link này sẽ hết hạn sau 1 giờ. Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này hoặc liên hệ với chúng tôi ngay.</p>
                            </div>
                        </div>
                        <div style='background: #f8f9fa; padding: 20px; text-align: center; border-radius: 0 0 10px 10px;'>
                            <p style='margin: 0; color: #666; font-size: 12px;'>© 2024 FirstAid+. Mọi quyền được bảo lưu.</p>
                        </div>
                    </div>";
                await _emailService.SendEmailAsync(model.Email, "[FirstAid+] Yêu cầu đặt lại mật khẩu", message);
            }

            // Always redirect to confirmation to prevent email enumeration
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ViewBag.Error = "Invalid password reset token";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ViewModels.ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (result)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            ViewBag.Error = "Token không hợp lệ hoặc đã hết hạn.";
            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
    }

