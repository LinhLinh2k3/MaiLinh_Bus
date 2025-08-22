using System.ComponentModel.DataAnnotations;
namespace NhaXeMaiLinh.ViewModels
{
    public class InvoiceLookupViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã hóa đơn")]
        public string InvoiceCode { get; set; }
        [Required]
        public string ReCaptchaToken { get; set; }
    }
}
