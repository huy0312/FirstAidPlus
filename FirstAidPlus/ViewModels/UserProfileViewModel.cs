using System.ComponentModel.DataAnnotations;

namespace FirstAidPlus.ViewModels
{
    public class UserProfileViewModel
    {
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; } // Read-only

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }

        [Display(Name = "Giới thiệu bản thân")]
        public string? Bio { get; set; }
    }
}
