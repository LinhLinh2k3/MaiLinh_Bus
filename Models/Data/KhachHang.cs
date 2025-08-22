using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class KhachHang
    {
        [Key]
        public string KhachHangID { get; set; } = Guid.NewGuid().ToString();
        public string? HoTen { get; set; }
        public string? SDT { get; set; }
        public string? Email { get; set; }
        public string? CCCD { get; set; }
        public string? DiaChi { get; set; }
        public string? HangThanhVien { get; set; }

        // Khóa ngoại để liên kết với bảng AppUser
        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
