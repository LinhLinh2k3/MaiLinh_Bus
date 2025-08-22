using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Models.Data
{
    public class TinTuc
    {
        [Key]
        public string TinTucId { get; set; }

        [DataType(DataType.Text)]
        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ThoiGian { get; set; }

        public string? Url { get; set; }

        public string? AnhBia { get; set; }

        public string NhanVienID { get; set; }

        // Link
        [ForeignKey(nameof(NhanVienID))]
        public virtual NhanVien NhanVien { get; set; }
        
        public virtual ICollection<FileTinTuc> FileTinTuc { get; set; }

    }
}
