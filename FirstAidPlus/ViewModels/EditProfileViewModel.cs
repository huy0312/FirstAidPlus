using System.ComponentModel.DataAnnotations;

namespace FirstAidPlus.ViewModels
{
    public class EditProfileViewModel
    {
        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? ExistingAvatarUrl { get; set; }
        public Microsoft.AspNetCore.Http.IFormFile? AvatarFile { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        public string? FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Giới thiệu bản thân")]
        public string? Bio { get; set; }
    }
}
