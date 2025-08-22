using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Admin.ViewModels;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RouteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RouteController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("/admin/route/danh-sach-lich-trinh")]
        public async Task<IActionResult> Index(DateTime? date)
        {

            DateOnly filterDate = DateOnly.FromDateTime(date ?? DateTime.Today); // Chuyển DateTime thành DateOnly

            var lichtrinh = await _context.LichTrinhs
                .Include(lt => lt.Xe)
                .Include(lt => lt.TuyenDuong)
                .Where(lt => lt.NgayKhoiHanh == filterDate) // Lọc theo ngày khởi hành
                .ToListAsync();

            ViewData["FilterDate"] = filterDate.ToString("yyyy-MM-dd"); // Đặt giá trị ngày cho view

            return View(lichtrinh);

        }

        private async Task<string> GenerateLichTrinhIdAsync()
        {
            var lastLichTrinh = await _context.LichTrinhs
                .OrderByDescending(lt => lt.LichTrinhId)
                .FirstOrDefaultAsync();

            int nextIdNumber = 1; // Giá trị mặc định khi không có Lịch Trình nào

            if (lastLichTrinh != null && lastLichTrinh.LichTrinhId.Length > 2)
            {
                // Tách phần số từ LichTrinhId
                string lastIdNumberStr = lastLichTrinh.LichTrinhId.Substring(2);
                if (int.TryParse(lastIdNumberStr, out int lastIdNumber))
                {
                    nextIdNumber = lastIdNumber + 1; // Tăng giá trị lên 1
                }
            }
            return $"LT{nextIdNumber:D3}";
        }


        [HttpGet]
        [Route("/admin/route/tao-lich-trinh")]
        public async Task<IActionResult> Create()
        {
            var model = new LichTrinhViewModel
            {
                DanhSachXe = await _context.Xes.Where(x => x.IsDeleted).ToListAsync(),
                //DanhSachTuyenDuong = await _context.TuyenDuongs.ToListAsync()
                DanhSachTuyenDuong = await _context.TuyenDuongs.Where(t => t.isEnabled == true).ToListAsync() // Lọc các tuyến đường đang hoạt động
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/route/tao-lich-trinh")]
        public async Task<IActionResult> Create(LichTrinhViewModel model)
        {
            // Xóa các giá trị không cần thiết khỏi ModelState để tránh lỗi xác thực
            ModelState.Remove("DanhSachXe");
            ModelState.Remove("DanhSachTuyenDuong");

            if (ModelState.IsValid)
            {
                try
                {
                    // Chuyển đổi các giá trị từ ViewModel về kiểu phù hợp với database
                    var lichTrinh = new LichTrinh
                    {
                        LichTrinhId = await GenerateLichTrinhIdAsync(), // Tạo ID tự động
                        XeId = model.XeId,
                        TuyenDuongId = model.TuyenDuongId,
                        GioKhoiHanh = TimeOnly.Parse(model.GioKhoiHanh), // Chuyển đổi từ string sang TimeOnly
                        GioDen = TimeOnly.Parse(model.GioDen),           // Chuyển đổi từ string sang TimeOnly
                        NgayKhoiHanh = DateOnly.Parse(model.NgayKhoiHanh), // Chuyển đổi từ string sang DateOnly
                        NgayDen = DateOnly.Parse(model.NgayDen),           // Chuyển đổi từ string sang DateOnly
                        GiaVe = model.GiaVe,
                        DieuChinhGiaVe = model.DieuChinhGiaVe
                    };

                    _context.LichTrinhs.Add(lichTrinh); // Thêm lịch trình vào database
                    await _context.SaveChangesAsync(); // Lưu thay đổi
                    TempData["SuccessMessage"] = "Thêm lịch trình thành công!";
                    return RedirectToAction("Index");
                }
                catch (FormatException ex)
                {
                    // Thêm thông báo lỗi nếu có vấn đề trong quá trình chuyển đổi
                    ModelState.AddModelError(string.Empty, "Định dạng dữ liệu không hợp lệ.");
                }
                catch (Exception ex)
                {
                    // Xử lý các lỗi khác
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình thêm lịch trình.");
                }
            }

            // Nếu có lỗi, tải lại danh sách xe và tuyến đường để hiển thị trong view
            model.DanhSachXe = await _context.Xes.Where(x => !x.IsDeleted).ToListAsync();
            model.DanhSachTuyenDuong = await _context.TuyenDuongs.Where(t => t.isEnabled == true).ToListAsync();

            // Trả lại ViewModel cho view
            return View(model);
        }
        //chuyeenr ddoi ben tay phai model
        [HttpGet]
        [Route("/admin/route/chi-tiet-lich-trinh/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var lichTrinh = await _context.LichTrinhs
        .Include(lt => lt.Xe)
        .Include(lt => lt.TuyenDuong)
        .FirstOrDefaultAsync(lt => lt.LichTrinhId == id);
          
            

            if (lichTrinh == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch trình.";
                return RedirectToAction("Index");
            }
            var model = new LichTrinhViewModel
            {
                XeId = lichTrinh.XeId,
                TuyenDuongId = lichTrinh.TuyenDuongId,
                GioKhoiHanh = lichTrinh.GioKhoiHanh.ToString("HH:mm"), // Chuyển sang định dạng HH:mm
                GioDen = lichTrinh.GioDen.ToString("HH:mm"),           // Chuyển sang định dạng HH:mm
                NgayKhoiHanh = lichTrinh.NgayKhoiHanh.ToString("yyyy-MM-dd"), // Định dạng ngày
                NgayDen = lichTrinh.NgayDen.ToString("yyyy-MM-dd"),           // Định dạng ngày
                GiaVe = lichTrinh.GiaVe,
                DieuChinhGiaVe = lichTrinh.DieuChinhGiaVe,
                DanhSachXe = await _context.Xes.Where(x => x.IsDeleted).ToListAsync(),
                DanhSachTuyenDuong = await _context.TuyenDuongs.Where(t => t.isEnabled == true).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/route/chi-tiet-lich-trinh/{id}")]
        public async Task<IActionResult> Edit(string id, LichTrinhViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lichTrinh = await _context.LichTrinhs.FindAsync(id);
                if (lichTrinh == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy lịch trình.";
                    return RedirectToAction("Index");
                }

                try
                {
                    // Cập nhật thông tin, chuyển đổi nếu cần thiết
                    lichTrinh.XeId = model.XeId;
                    lichTrinh.TuyenDuongId = model.TuyenDuongId;

                    // Chuyển đổi từ TimeSpan sang TimeOnly
                    lichTrinh.GioKhoiHanh = TimeOnly.FromTimeSpan(TimeSpan.Parse(model.GioKhoiHanh));
                    lichTrinh.GioDen = TimeOnly.FromTimeSpan(TimeSpan.Parse(model.GioDen));

                    // Chuyển đổi từ string sang DateOnly
                    lichTrinh.NgayKhoiHanh = DateOnly.Parse(model.NgayKhoiHanh);
                    lichTrinh.NgayDen = DateOnly.Parse(model.NgayDen);

                    lichTrinh.GiaVe = model.GiaVe;
                    lichTrinh.DieuChinhGiaVe = model.DieuChinhGiaVe;

                    // Lưu thay đổi
                    _context.LichTrinhs.Update(lichTrinh);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Chỉnh sửa lịch trình thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                    return RedirectToAction("Index");
                }
            }

            // Nếu có lỗi xác thực, tải lại danh sách để hiển thị
            model.DanhSachXe = await _context.Xes.Where(x => !x.IsDeleted).ToListAsync();
            model.DanhSachTuyenDuong = await _context.TuyenDuongs.Where(t => t.isEnabled == true).ToListAsync(); 
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/route/xoa-lich-trinh/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var lichTrinh = await _context.LichTrinhs.FindAsync(id);
            if (lichTrinh == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch trình cần xóa.";
                return RedirectToAction("Index");
            }

            try
            {
                _context.LichTrinhs.Remove(lichTrinh);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa lịch trình thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi xóa lịch trình: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
