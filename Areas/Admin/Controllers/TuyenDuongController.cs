using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Admin.ViewModels;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TuyenDuongController : Controller
    {
   
        private readonly ApplicationDbContext _context;

        public TuyenDuongController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/admin/tuyenduong/danh-sach-tuyen-duong")]
        public async Task<IActionResult> Index()
        {
            var tuyenduong = await _context.TuyenDuongs
                                   .Where(t => t.isEnabled == true)  // Lọc chỉ những tuyến đường có isEnabled là true
                                   .ToListAsync();
            return View(tuyenduong);
        }
        private bool TuyenDuongExists(string id)
        {
            return _context.TuyenDuongs.Any(e => e.TuyenDuongID == id);
        }
        [HttpGet]
        [Route("/chi-tiet-tuyen-duong/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tuyenduong = await _context.TuyenDuongs.FindAsync(id);
            if (tuyenduong == null)
            {
                return NotFound();
            }
            var model = new TuyenDuongViewModel
            {
                TuyenDuongID = tuyenduong.TuyenDuongID,
                TenTuyenDuong = tuyenduong.TenTuyenDuong,
                DiemDi = tuyenduong.DiemDi,
                DiemDen = tuyenduong.DiemDen,
                QuangDuong = tuyenduong.QuangDuong
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/chi-tiet-tuyen-duong/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("TuyenDuongID,TenTuyenDuong,DiemDi,DiemDen,QuangDuong")] TuyenDuongViewModel model)
        {
            if (id != model.TuyenDuongID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tuyenduong = await _context.TuyenDuongs.FindAsync(id);
                    if (tuyenduong == null)
                    {
                        return NotFound();
                    }
                    tuyenduong.TenTuyenDuong = model.TenTuyenDuong;
                    tuyenduong.DiemDi = model.DiemDi;
                    tuyenduong.DiemDen = model.DiemDen;
                    tuyenduong.QuangDuong = model.QuangDuong;

                    _context.Update(tuyenduong);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TuyenDuongExists(model.TuyenDuongID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [Route("/admin/them-tuyen-duong")]
        public IActionResult Create()
        {
            var model = new TuyenDuongViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/them-tuyen-duong")]
        public async Task<IActionResult> Create(TuyenDuongViewModel model)
        {
            // Loại bỏ trạng thái không hợp lệ của TuyenDuongID vì nó sẽ được gán tự động
            ModelState.Remove("TuyenDuongID");

            if (ModelState.IsValid)
            {
                var maxId = await _context.TuyenDuongs
                    .OrderByDescending(t => t.TuyenDuongID)
                    .Select(t => t.TuyenDuongID)
                    .FirstOrDefaultAsync();

                string tdID = "TD" + (_context.TuyenDuongs.Count() + 1).ToString("D3"); 

                var newTuyenDuong = new TuyenDuong
                {
                    TuyenDuongID = tdID, // Tạo ID tự động tăng dần
                    TenTuyenDuong = model.TenTuyenDuong,
                    DiemDi = model.DiemDi,
                    DiemDen = model.DiemDen,
                    QuangDuong = model.QuangDuong,
                    isEnabled = true
                };

                _context.TuyenDuongs.Add(newTuyenDuong);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm tuyến đường thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/xoa-tuyen-duong/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Tìm tuyến đường theo ID
                var tuyenduong = await _context.TuyenDuongs.FindAsync(id);

                // Kiểm tra nếu không tìm thấy tuyến đường
                if (tuyenduong == null)
                {
                    TempData["ErrorMessage"] = "Xóa thất bại: Không tìm thấy tuyến đường.";
                    return RedirectToAction(nameof(Index));
                }

                // Đánh dấu tuyến đường là không hoạt động
                tuyenduong.isEnabled = false;

                // Cập nhật cơ sở dữ liệu
                _context.TuyenDuongs.Update(tuyenduong);
                await _context.SaveChangesAsync();

                // Hiển thị thông báo thành công
                TempData["SuccessMessage"] = "Xóa tuyến đường thành công!";
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, hiển thị thông báo lỗi
                TempData["ErrorMessage"] = "Xóa thất bại: Đã xảy ra lỗi khi xóa tuyến đường.";
            }

            return RedirectToAction(nameof(Index));
        }

        [Route("/admin/tuyen-duong/danh-sach-da-xoa")]
        public async Task<IActionResult> dsTuyenduongxoa()
        {
            var tuyenduong = await _context.TuyenDuongs
                                   .Where(t => t.isEnabled == false)  // Lọc chỉ những tuyến đường có isEnabled là true
                                   .ToListAsync();
            return View(tuyenduong);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Route("/admin/nhanvien/khoi-phuc-nhan-vien/{id}")]
        //public async Task<IActionResult> Restore(string id)
        //{
        //    var nhanVien = await _context.NhanViens.FindAsync(id);
        //    if (nhanVien == null)
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
        //        return RedirectToAction("nhanvienxoa");
        //    }

        //    var user = await _userManager.FindByIdAsync(nhanVien.AppUserId);
        //    if (user != null)
        //    {
        //        user.isEnabled = true;
        //        await _userManager.UpdateAsync(user);
        //        TempData["SuccessMessage"] = "Khôi phục nhân viên thành công!";
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy tài khoản người dùng.";
        //    }

        //    return RedirectToAction("nhanvienxoa");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/khoi-phuc-tuyen-duong/{id}")]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                // Tìm tuyến đường theo ID
                var tuyenduong = await _context.TuyenDuongs.FindAsync(id);
                if (tuyenduong == null)
                {
                    TempData["ErrorMessage"] = "Khôi phục thất bại: Không tìm thấy tuyến đường.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {

                    tuyenduong.isEnabled = true;
                    _context.TuyenDuongs.Update(tuyenduong);
                    await _context.SaveChangesAsync();

                    // Thông báo thành công
                    TempData["SuccessMessage"] = "Khôi phục tuyến đường thành công!";
                }
               
            }
            catch (Exception ex)
            {
                // Nếu có lỗi xảy ra, hiển thị thông báo lỗi
                TempData["ErrorMessage"] = "Khôi phục thất bại: Đã xảy ra lỗi khi khôi phục tuyến đường.";
            }

            // Điều hướng về danh sách tuyến đường
            return RedirectToAction("dsTuyenduongxoa");
        }


    }

}
