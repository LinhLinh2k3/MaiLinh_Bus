using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Areas.Staff.Models
{
    public class ThemKhachHang
    {
        public string HoTen { get; set; }

        [Phone]
        public string SDT { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string CCCD { get; set; }
        public string DiaChi { get; set; }
        public string HangThanhVien { get; set; }
    }
}