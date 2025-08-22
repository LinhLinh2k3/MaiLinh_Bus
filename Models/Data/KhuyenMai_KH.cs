using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class KhuyenMai_KH
    {
        public string KhuyenMaiID { get; set; }
        public string KhachHangID { get; set; }

        [ForeignKey(nameof(KhuyenMaiID))]
        public KhuyenMai KhuyenMai { get; set; }

        [ForeignKey(nameof(KhachHangID))]
        public KhachHang KhachHang { get; set; }

    }
}
