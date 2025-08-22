using Microsoft.AspNetCore.Identity;
namespace NhaXeMaiLinh.Models.Data
{
    public class AppUser: IdentityUser
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public bool? isEnabled { get; set; }

        public KhachHang? KhachHang { get; set; }
        public NhanVien? NhanVien { get; set; }
    }
}
