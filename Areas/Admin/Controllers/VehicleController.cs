using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Admin.ViewModels;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Route("/admin/danh-sach-xe")]
        public async Task<IActionResult> Index()
        {
            var xes = await _context.Xes
                .Include(x => x.LoaiXe)
                .Where(x => x.IsDeleted == true) 
                .ToListAsync();
            return View(xes);
        }
        // GET: /admin/xe/tao-xe
        [HttpGet]
        [Route("/admin/xe/tao-xe")]
        public async Task<IActionResult> Create()
        {
            var model = new XeViewModel
            {
                LoaiXes = await _context.LoaiXes.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/xe/tao-xe")]
        public async Task<IActionResult> Create(XeViewModel model)
        {
            ModelState.Remove("LoaiXes");
            if (ModelState.IsValid)
            {
                // Tạo đối tượng Xe mới
                var xe = new Xe
                {
                    BienSo = model.BienSo,
                    LoaiXeId = model.LoaiXeId,
                    TinhTrang = model.TinhTrang,
                    IsDeleted = true
                };

                // Thêm xe vào bảng Xes
                _context.Xes.Add(xe);
                await _context.SaveChangesAsync();

                // Lấy số lượng ghế của loại xe từ bảng LoaiXes
                var loaiXe = await _context.LoaiXes
                    .FirstOrDefaultAsync(lx => lx.LoaiXeID == model.LoaiXeId);

                if (loaiXe != null)
                {
                    // Tạo danh sách ghế
                    var gheList = new List<Ghe>();
                    for (int i = 1; i <= loaiXe.SLGhe; i++)
                    {
                        gheList.Add(new Ghe
                        {
                            XeID = xe.XeID,
                            GheID = i,  // GhếID có thể là i, hoặc bạn có thể chọn logic khác để tạo GhếID.
                            TenGhe = i < 10 ? $"A0{i}" : $"A{i}", // Kiyoshi Kun: Cưa đôi số lượng ghế để chia thành dạng Axx và Bxx (xx là số thứ tự)
                            TrangThai = "trống" // Kiyoshi Kun: trạng thái là "trống" nha, check lại Model, đã có ghi chú oy
                        });
                    }

                    // Thêm các ghế vào DbContext
                    _context.Ghes.AddRange(gheList);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Thêm xe mới thành công!";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, load lại danh sách loại xe và trả về view
            model.LoaiXes = await _context.LoaiXes.ToListAsync();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/xe/xoa-xe/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy xe.";
                return RedirectToAction("Index");
            }

            // Đánh dấu xe là đã xóa (xóa mềm)
            xe.IsDeleted = false;
            _context.Xes.Update(xe);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa xe thành công!";
            return RedirectToAction("Index");
        }
        [Route("/admin/xe/chi-tiet-xe/{id}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Tìm xe theo id
            var xe = await _context.Xes
                .Include(x => x.LoaiXe) // Bao gồm thông tin về loại xe
                .FirstOrDefaultAsync(x => x.XeID == id);

            if (xe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy xe.";
                return RedirectToAction("Index");
            }

            // Chuẩn bị ViewModel để hiển thị trên giao diện
            var model = new XeViewModel
            {
                BienSo = xe.BienSo,
                LoaiXeId = xe.LoaiXeId,
                TinhTrang = xe.TinhTrang,
                LoaiXes = await _context.LoaiXes.ToListAsync() // Lấy danh sách loại xe cho combobox
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/xe/chi-tiet-xe/{id}")]
        public async Task<IActionResult> Edit(int id, XeViewModel model)
        {
            // Xóa ModelState của LoaiXes để tránh lỗi xác thực
            ModelState.Remove("LoaiXes");

            if (ModelState.IsValid)
            {
                // Tìm xe trong cơ sở dữ liệu
                var xe = await _context.Xes.FindAsync(id);
                if (xe == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy xe.";
                    return RedirectToAction("Index");
                }

                // Cập nhật thông tin (không thay đổi XeID)
                xe.BienSo = model.BienSo;
                xe.LoaiXeId = model.LoaiXeId;
                xe.TinhTrang = model.TinhTrang;

                _context.Xes.Update(xe);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật xe thành công!";
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, load lại danh sách loại xe và trả về view
            model.LoaiXes = await _context.LoaiXes.ToListAsync();
            return View(model);
        }
        [Route("/admin/danh-sach-xe-da-xoa")]
        public async Task<IActionResult> xedaxoa()
        {

           var xes = await _context.Xes
        .Include(x => x.LoaiXe)
        .Where(x => x.IsDeleted == false) 
        .ToListAsync();
            return View(xes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/xe/khôi-phục/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var xe = await _context.Xes.FindAsync(id);
            if (xe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy xe.";
                return RedirectToAction("xedaxoa");
            }

            xe.IsDeleted = true;
            _context.Xes.Update(xe);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Khôi phục xe thành công!";
            return RedirectToAction("xedaxoa"); // Trả về view danh sách xe đã xóa
        }
    }
}
