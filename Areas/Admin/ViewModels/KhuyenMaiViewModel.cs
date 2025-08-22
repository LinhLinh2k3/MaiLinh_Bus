using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class KhuyenMaiViewModel
    {
        public string KhuyenMaiID { get; set; }

        [Required(ErrorMessage = "Tên khuyến mãi là bắt buộc.")]
        public string TenKhuyenMai { get; set; }

        [Required(ErrorMessage = "Loại khuyến mãi là bắt buộc.")]
        public string LoaiKhuyenMai { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        public DateTime NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Giá trị giảm là bắt buộc.")]
        public decimal GiaTriGiam { get; set; }

        [Required(ErrorMessage = "Điều kiện áp dụng là bắt buộc.")]
        public int DieuKienApDung { get; set; }

        [Required(ErrorMessage = "Trạng thái thanh toán là bắt buộc.")]
        public string TrangThaiThanhToan { get; set; }
    }
}
