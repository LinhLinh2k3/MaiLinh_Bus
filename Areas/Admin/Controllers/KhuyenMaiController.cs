using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Admin.ViewModels;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhuyenMaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhuyenMaiController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("/admin/khuyenmai/danh-sach-khuyen-mai")]
        public async Task<IActionResult> Index()
        {
            var km = await _context.KhuyenMais.ToListAsync();
            return View(km);
        }
        // Hàm tạo mã khuyến mãi tự động (VD: KM001, KM002,...)
        private async Task<string> GenerateKhuyenMaiIDAsync()
        {
            var lastKhuyenMai = await _context.KhuyenMais
                .OrderByDescending(km => km.KhuyenMaiID)
                .FirstOrDefaultAsync();

            int nextID = 1; // Mặc định là 1 nếu không có bản ghi
            if (lastKhuyenMai != null && lastKhuyenMai.KhuyenMaiID.Length > 2)
            {
                string lastID = lastKhuyenMai.KhuyenMaiID.Substring(2); // Lấy phần số
                if (int.TryParse(lastID, out int lastIDNumber))
                {
                    nextID = lastIDNumber + 1; // Tăng giá trị lên 1
                }
            }

            return $"KM{nextID:D3}"; // Định dạng thành "KM001", "KM002",...
        }


        [HttpGet]
        [Route("/admin/khuyen-mai/tao-khuyen-mai")]
        public IActionResult Create()
        {
            return View(new KhuyenMaiViewModel());
        }


        [HttpPost]
        [Route("/admin/khuyen-mai/tao-khuyen-mai")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuyenMaiViewModel model)
        {
            ModelState.Remove("KhuyenMaiID");
            if (ModelState.IsValid)
            {
                // Tạo mã khuyến mãi tự động
                string newKhuyenMaiID = await GenerateKhuyenMaiIDAsync();

                var khuyenMai = new KhuyenMai
                {
                    KhuyenMaiID = newKhuyenMaiID, // Sử dụng mã tự động
                    TenKhuyenMai = model.TenKhuyenMai,
                    LoaiKhuyenMai = model.LoaiKhuyenMai,
                    NgayBatDau = model.NgayBatDau,
                    NgayKetThuc = model.NgayKetThuc,
                    GiaTriGiam = model.GiaTriGiam,
                    DieuKienApDung = model.DieuKienApDung,
                    TrangThaiThanhToan = model.TrangThaiThanhToan
                };

                _context.KhuyenMais.Add(khuyenMai);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm khuyến mãi thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }
        // Chỉnh sửa khuyến mãi (GET)
        [HttpGet]
        [Route("/admin/khuyen-mai/chi-tiet-khuyen-mai/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            if (khuyenMai == null) return NotFound();

            var model = new KhuyenMaiViewModel
            {
                KhuyenMaiID = khuyenMai.KhuyenMaiID,
                TenKhuyenMai = khuyenMai.TenKhuyenMai,
                LoaiKhuyenMai = khuyenMai.LoaiKhuyenMai,
                NgayBatDau = khuyenMai.NgayBatDau,
                NgayKetThuc = khuyenMai.NgayKetThuc,
                GiaTriGiam = khuyenMai.GiaTriGiam,
                DieuKienApDung = khuyenMai.DieuKienApDung,
                TrangThaiThanhToan = khuyenMai.TrangThaiThanhToan
            };

            return View(model);
        }

        // Chỉnh sửa khuyến mãi (POST)
        [HttpPost]
        [Route("/admin/khuyen-mai/chi-tiet-khuyen-mai/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, KhuyenMaiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var khuyenMai = await _context.KhuyenMais.FindAsync(id);
                if (khuyenMai == null) return NotFound();

                khuyenMai.TenKhuyenMai = model.TenKhuyenMai;
                khuyenMai.LoaiKhuyenMai = model.LoaiKhuyenMai;
                khuyenMai.NgayBatDau = model.NgayBatDau;
                khuyenMai.NgayKetThuc = model.NgayKetThuc;
                khuyenMai.GiaTriGiam = model.GiaTriGiam;
                khuyenMai.DieuKienApDung = model.DieuKienApDung;
                khuyenMai.TrangThaiThanhToan = model.TrangThaiThanhToan;

                _context.KhuyenMais.Update(khuyenMai);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Chỉnh sửa khuyến mãi thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // Xóa khuyến mãi
        [HttpPost]
        [Route("/admin/khuyen-mai/xoa/{id}")]
     
        public async Task<IActionResult> Delete(string id)
        {
            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            if (khuyenMai == null) return NotFound();

            _context.KhuyenMais.Remove(khuyenMai);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa khuyến mãi thành công!";
            return RedirectToAction("Index");
        }



    }
}
