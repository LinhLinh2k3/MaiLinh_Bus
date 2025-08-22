
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class LichSuDoiGhe
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int DoiGheID { get; set; }
        public string ChiTietVeID { get; set; }
        public int GheCu { get; set; }
        public int GheMoi { get; set; }
        public DateTime NgayDoi { get; set; }
        public string LyDoDoi { get; set; }
        public decimal ChenhLechGia { get; set; }
        public string? NhanVienID { get; set; }
        public string GiaoDichID { get; set; }


        public ChiTietVeDat ChiTietVeDat { get; set; }

        [ForeignKey(nameof(NhanVienID))]
        public NhanVien NhanVien { get; set; }

        [ForeignKey(nameof(GiaoDichID))]
        public LichSuGiaoDich LichSuGiaoDich { get; set; }
    }
}
