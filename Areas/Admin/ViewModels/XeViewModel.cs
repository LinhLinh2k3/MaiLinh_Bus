using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class XeViewModel
    {
        [Required(ErrorMessage = "Biển số là bắt buộc.")]
        [StringLength(10, ErrorMessage = "Biển số không được vượt quá 10 ký tự.")]
        public string BienSo { get; set; }

        [Required(ErrorMessage = "Loại xe là bắt buộc.")]
        public string LoaiXeId { get; set; }

        [Required(ErrorMessage = "Tình trạng là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Tình trạng không được vượt quá 50 ký tự.")]
        public string TinhTrang { get; set; }

      
        [BindNever]  
        public List<LoaiXe> LoaiXes { get; set; }
    }
}