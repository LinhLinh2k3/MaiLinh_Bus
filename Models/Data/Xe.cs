using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaXeMaiLinh.Models.Data
{
    public class Xe
    {
        [Key]
        public int XeID { get; set; }
        public string BienSo { get; set; }
        public string LoaiXeId { get; set; }
        public string TinhTrang { get; set; }

        [ForeignKey(nameof(LoaiXeId))]
        public virtual LoaiXe LoaiXe { get; set; }
        public bool IsDeleted { get; set; } // Đánh dấu xe có bị xóa hay không

    }
}