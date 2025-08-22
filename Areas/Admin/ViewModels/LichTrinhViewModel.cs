using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.ViewModels
{
    public class LichTrinhViewModel
    {
        [Required(ErrorMessage = "Xe là bắt buộc")]
        public int XeId { get; set; }

        [Required(ErrorMessage = "Tuyến đường là bắt buộc")]
        public string TuyenDuongId { get; set; }

        [Required(ErrorMessage = "Giờ khởi hành là bắt buộc")]
        public string GioKhoiHanh { get; set; }

        [Required(ErrorMessage = "Giờ đến là bắt buộc")]
     
        public string GioDen { get; set; }

        [Required(ErrorMessage = "Ngày khởi hành là bắt buộc")]
       
        public string NgayKhoiHanh { get; set; }

        [Required(ErrorMessage = "Ngày đến là bắt buộc")]
        public string NgayDen { get; set; }

        [Required(ErrorMessage = "Giá vé là bắt buộc")]
        public float GiaVe { get; set; }

        [Required(ErrorMessage = "Điều chỉnh giá vé là bắt buộc ")]
        public int DieuChinhGiaVe { get; set; }

        public List<Xe> DanhSachXe { get; set; }
        public List<TuyenDuong> DanhSachTuyenDuong { get; set; }
    }
}
