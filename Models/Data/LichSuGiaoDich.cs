using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class LichSuGiaoDich
    {
        [Key]
        public string GiaoDichID { get; set; }
        public string VeID { get; set; }
        public string? LoaiGiaoDich { get; set; } // "online", "offline", "cancel"
        public string ChiTiet { get; set; }
        public DateTime NgayGiaoDich { get; set; }
        public string? NhanVienID { get; set; } // null với thanh toán online (không làm từ nhân viên - tự làm)
        public string? TrangThaiGiaoDich { get; set; } // "chờ thanh toán", "đã thanh toán" , "chưa thanh toán"

        [ForeignKey(nameof(VeID))]
        public VeXe VeXe { get; set; }

        [ForeignKey(nameof(NhanVienID))]
        public NhanVien NhanVien { get; set; }
    }
}
