using System.ComponentModel.DataAnnotations.Schema;
namespace NhaXeMaiLinh.Models.Data
{
	public class Ghe
	{
		public int GheID { get; set; }
		public string TenGhe { get; set; }
		public int XeID { get; set; }
		public string TrangThai { get; set; } // "trống", "mua"

        [ForeignKey(nameof(XeID))]
		public Xe Xe { get; set; }
	}
}