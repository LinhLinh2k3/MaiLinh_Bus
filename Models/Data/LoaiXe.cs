using System.ComponentModel.DataAnnotations;
namespace NhaXeMaiLinh.Models.Data
{
	public class LoaiXe
	{
		[Key]
		public string LoaiXeID { get; set; }
		public string TenLoaiXe { get; set; }
		public string HangXe { get; set; }
		public int? SLGhe {  get; set; }
	}
}