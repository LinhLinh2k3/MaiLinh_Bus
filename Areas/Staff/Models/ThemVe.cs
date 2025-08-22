using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Staff.Models
{
    public class VeModel
    {
        public string VeID { get; set; } = Guid.NewGuid().ToString();
        public string KhachHangID { get; set; }
        public string LichTrinhID { get; set; }
        public string KhuyenMaiID { get; set; }
        public decimal TongGiaVe { get; set; }
        public ICollection<ChiTietVeDat>? ChiTietVeDats { get; set; }
    }
}
