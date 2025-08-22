using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Models.Data
{
    public class FileTinTuc
    {
        [Key]
        public string FileId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string TenFile { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Loai { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FilePath { get; set; }

        // FK
        [Required]
        public string TinTucId { get; set; }

        [ForeignKey(nameof(TinTucId))]
        public virtual TinTuc TinTuc { get; set; }
    }
}
