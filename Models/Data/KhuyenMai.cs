using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Models.Data
{
    public class KhuyenMai
    {
        [Key]
        public string KhuyenMaiID { get; set; }
        public string TenKhuyenMai { get; set; }
        public string LoaiKhuyenMai { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc {  get; set; }
        public decimal GiaTriGiam { get; set; }
        public int DieuKienApDung { get; set; }
        public string TrangThaiThanhToan { get; set; } // dang su dung, khong su dụng
    }
}