using NhaXeMaiLinh.Data;
using System.ComponentModel.DataAnnotations;
using NhaXeMaiLinh.Models.Data; // Đảm bảo import đúng namespace chứa class TuyenDuong
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;




namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class TuyenDuongViewModel
    {
        //[BindNever]
        public string TuyenDuongID { get; set; }

        [Required(ErrorMessage = "Tên tuyến đường là bắt buộc.")]
      
        public string TenTuyenDuong { get; set; }

        [Required(ErrorMessage = "Điểm đi là bắt buộc.")]
       
        public string DiemDi { get; set; }

        [Required(ErrorMessage = "Điểm đến là bắt buộc.")]
       
        public string DiemDen { get; set; }

        [Required(ErrorMessage = "Quãng đường là bắt buộc.")]
        [Range(0, float.MaxValue, ErrorMessage = "Quãng đường phải là một số dương.")]
        public float QuangDuong { get; set; }
    }
}
