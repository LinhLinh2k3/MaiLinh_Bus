using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class VeXe
    {
        [Key]
        public string VeID { get; set; }
        public string KhachHangID { get; set; }
        public string LichTrinhID { get; set; }
        public string KhuyenMaiID { get; set; }
        public decimal TongGiaVe { get; set; }
        public bool? isDelete { get; set; } = false;

        [ForeignKey(nameof(KhachHangID))]
        public KhachHang KhachHang { get; set; }
        [ForeignKey(nameof(LichTrinhID))]
        public LichTrinh LichTrinh { get; set; }
        [ForeignKey(nameof(KhuyenMaiID))]
        public KhuyenMai? KhuyenMai { get; set; }

        public ICollection<ChiTietVeDat> ChiTietVeDats { get; set; }
    }
}