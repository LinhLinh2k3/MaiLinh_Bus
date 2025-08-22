using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.ViewModels
{
    public class ForgotPasswordVM
    {
   
        [Required(ErrorMessage = "Vui lòng nhập Email!! ")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
