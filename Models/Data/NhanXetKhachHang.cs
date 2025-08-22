using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class NhanXetKhachHang
    {
        [Key]
        public int NhanXetID { get; set; }
        public string KhachHangID { get; set; }

        public string LichTrinhID { get; set; }
        public int DanhGia { get; set; }
        public string NhanXet { get; set; }

        [ForeignKey(nameof(KhachHangID))]
        public KhachHang KhachHang { get; set; }

    }
}
