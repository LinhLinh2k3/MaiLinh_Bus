using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class ChiTietVeDat
    {
        [Key]
        public string ChiTietVeID { get; set; }
        public string VeID { get; set; }
        public int SoGhe { get; set; }
        public int XeID { get; set; }
        public decimal GiaGhe { get; set; }
        public string TinhTrangGhe { get; set; } // "còn trống", "đang giữ", "đã mua", "đã hủy"
        public DateTime? NgayDat { get; set; }

        [ForeignKey(nameof(VeID))]
        public virtual VeXe VeXe { get; set; }

        [ForeignKey(nameof(SoGhe) + "," + nameof(XeID))]
        public virtual Ghe Ghe { get; set; }
    }
}