using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FirstAidPlus.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "{0} phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // === Expert Registration Fields ===
        public bool IsExpert { get; set; } = false;

        [Display(Name = "Học hàm / Học vị")]
        public string? ExpertTitle { get; set; }

        [Display(Name = "Nơi công tác")]
        public string? Workplace { get; set; }

        [Display(Name = "Số năm kinh nghiệm")]
        public int? ExperienceYears { get; set; }

        [Display(Name = "Chứng chỉ chuyên môn")]
        public IFormFile? CertificateFile { get; set; }
    }
}
