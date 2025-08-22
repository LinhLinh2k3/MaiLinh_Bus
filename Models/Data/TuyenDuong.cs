using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Models.Data
{
    public class TuyenDuong
    {
        [Key]
        public string TuyenDuongID { get; set; }
        public string TenTuyenDuong { get; set; }
        public string DiemDi { get; set; }
        public string DiemDen { get; set; }
        public float QuangDuong { get; set; }
        public bool? isEnabled { get; set; }
    }
}
