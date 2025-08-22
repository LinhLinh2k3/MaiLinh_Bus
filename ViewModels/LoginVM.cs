using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người dùng hoặc Email!! ")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự. ")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu ")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Display(Name = "Nhớ tài khoản")]
        public bool RememberMe { get; set; }
    }
}
