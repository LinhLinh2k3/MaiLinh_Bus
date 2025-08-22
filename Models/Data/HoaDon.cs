using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class HoaDon
    {
        [Key]
        public int HoaDonID { get; set; }
        public string VeID { get; set; }
        public string? NhanVienID { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public string PhuongThucThanhToan { get; set; } // "vnpay", "zalopay", "momo", "tiền mặt", chưa có
        public string TrangThaiThanhToan { get; set; } // "chờ thanh toán", "đã thanh toán" , "chưa thanh toán","đã hủy"

        [ForeignKey(nameof(VeID))]
        public VeXe VeXe { get; set; }

        [ForeignKey(nameof(NhanVienID))]
        public NhanVien NhanVien { get; set; }
    }

    //5,6
}
