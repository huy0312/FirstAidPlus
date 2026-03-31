using System.ComponentModel.DataAnnotations;

namespace FirstAidPlus.Areas.Admin.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được quá 50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? FullName { get; set; }
        
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? Phone { get; set; }
        
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public int RoleId { get; set; }
    }
}
