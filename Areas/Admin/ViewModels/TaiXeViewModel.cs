using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class TaiXeViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]

        [Phone]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa số.")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập CCCD.")]

        [Display(Name = "CCCD")]
        public string CCCD { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hạng bằng laí.")]
        [Display(Name = "Bằng lái xe")]
        public string BangLaiXe { get; set; }
    }
}
