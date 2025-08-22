using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.ViewModels
{
    public class UpdateProfileVM
    {
        [Required]
        public string HoTen { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? CCCD { get; set; }
        public string? DiaChi { get; set; }
        public string? HangThanhVien { get; set; }
    }
}
