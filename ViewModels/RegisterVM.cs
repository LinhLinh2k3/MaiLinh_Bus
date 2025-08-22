using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string? Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage= "Mật khẩu không trùng khớp")]
        //[Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }
    }
}
