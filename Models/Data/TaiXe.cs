using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Models.Data
{
    public class TaiXe
    {
        [Key]
        public string TaiXeID { get; set; }
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string CCCD { get; set; }
        public string BangLaiXe { get; set; }
    }
}
