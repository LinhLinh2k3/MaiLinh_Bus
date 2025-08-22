using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;
namespace NhaXeMaiLinh.Areas.APIs
{
    public class GetController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GetController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/thong-tin/lich-trinh/tien/{id?}")]
        [HttpGet]
        public IActionResult LayGiaTien(string id)
        {
            var dslichtrinh = _context.LichTrinhs.Include(l => l.TuyenDuong)
                .Include(l => l.VeXe).Include(l => l.Xe)
                .FirstOrDefault(v => v.LichTrinhId == id && v.Xe.IsDeleted == true);
            if (dslichtrinh == null) return Json(null);
            return Json(dslichtrinh.GiaVe);
        }

        [Route("/thong-tin/lich-trinh/giam-gia/{id?}")]
        [HttpGet]
        public IActionResult LayGiamGia(string id)
        {
            var dsgg = _context.KhuyenMais.First(km => km.KhuyenMaiID == id);
            if (dsgg == null) return Json(null);
            return Json(dsgg.GiaTriGiam);
        }

        [Route("/thong-tin/lich-trinh/ghe/{id?}")]
        [HttpGet]
        public IActionResult LayGhe(string id)
        {
            var dslichtrinh = _context.LichTrinhs.Include(l => l.Xe).FirstOrDefault(l => l.LichTrinhId == id && l.Xe.IsDeleted == true);
            if (dslichtrinh == null) return Json(null);

            var dsGhe = _context.Ghes.Include(g => g.Xe)
                .Where(g => g.XeID == dslichtrinh.XeId)
                .OrderBy(g => g.TenGhe).ToList();
            return Json(dsGhe);
        }

        [Route("/thong-tin/lich-trinh/query/{query?}")]
        [HttpGet]
        public IActionResult LayLichTrinh(string query)
        {
            if (string.IsNullOrEmpty(query)) return Json(null);
            
            var truyvan = _context.LichTrinhs.Include(l => l.TuyenDuong)
                .OrderBy(l => l.TuyenDuong.TenTuyenDuong)
                .Where(t => t.TuyenDuong.TenTuyenDuong.ToLower().Contains(query))
                .Take(5).ToList();
            return Json(truyvan);
        }

        [Route("/thong-tin/khach-hang/{query?}")]
        [HttpGet]
        public IActionResult LayKhachHang(string query)
        {
            if (string.IsNullOrEmpty(query)) return Json(null);

            var truyvan = _context.KhachHangs.OrderBy(kh => kh.HoTen)
                .Where(kh => kh.HoTen!.ToLower().Contains(query) ||
                        kh.SDT!.ToLower().Contains(query) ||
                        kh.CCCD!.ToLower().Contains(query))
                .Take(5).ToList();
            return Json(truyvan);
        }

    }
}
//restful api tương tác dữ liệu người dùng với sever