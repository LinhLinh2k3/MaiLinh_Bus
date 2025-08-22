using System.ComponentModel.DataAnnotations;

namespace NhaXeMaiLinh.ViewModels
{
    // ViewModel để lưu trữ dữ liệu khi người dùng thực hiện đổi mật khẩu
    public class ChangePasswordVM
    {
 
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } // Mật khẩu hiện tại của người dùng

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới để xác nhận")]
        [DataType(DataType.Password)] // Hiển thị dưới dạng ô nhập mật khẩu
        [Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp.")]
        public string ConfirmPassword { get; set; } // Xác nhận mật khẩu mới
    }
}
