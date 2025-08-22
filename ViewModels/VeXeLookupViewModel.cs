using System.ComponentModel.DataAnnotations;
namespace NhaXeMaiLinh.ViewModels
{
    public class VeXeLookupViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã vé")]
        public string MaVe { get; set; }
        [Required]
        public string ReCaptchaToken { get; set; }
    }
}
