using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class CreateNhanVienViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
      
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
      
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        // Các thuộc tính khác nếu cần
    }
}
