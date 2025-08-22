using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class LichTrinh
    {
        [Key]
        public string LichTrinhId { get; set; }
        public int XeId { get; set; }
        public string TuyenDuongId { get; set; }
        public TimeOnly GioKhoiHanh { get; set; }
        public TimeOnly GioDen {  get; set; }
        public DateOnly NgayKhoiHanh { get; set; }
        public DateOnly NgayDen { get; set; }
        public float GiaVe { get; set; }

        public int DieuChinhGiaVe { get; set; }

        [ForeignKey(nameof(TuyenDuongId))]
        public TuyenDuong TuyenDuong { get; set; }

        public Xe Xe { get; set; }

        public ICollection<VeXe> VeXe { get; set; }
    }
}
