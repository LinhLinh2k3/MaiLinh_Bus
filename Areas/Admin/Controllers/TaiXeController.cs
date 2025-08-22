using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using System.Linq;
using System.Threading.Tasks;
using NhaXeMaiLinh.Areas.Admin.ViewModels;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiXeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/admin/taixe/danh-sach-tai-xe")]
        public async Task<IActionResult> Index()
        {
            var taiXe = await _context.TaiXes.ToListAsync();
            return View(taiXe);
        }
        private bool TaiXeExists(string id)
        {
            return _context.TaiXes.Any(e => e.TaiXeID == id);
        }
        [HttpGet]
        [Route("/chi-tiet-tai-xe/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiXe = await _context.TaiXes.FindAsync(id);
            if (taiXe == null)
            {
                return NotFound();
            }

            return View(taiXe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("chi-tiet-tai-xe/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("TaiXeID,HoTen,SDT,CCCD,BangLaiXe")] TaiXe taiXe)
        {
            if (id != taiXe.TaiXeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiXe);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiXeExists(taiXe.TaiXeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(taiXe);
        }
        [HttpGet]
        [Route("/admin/them-tai-xe/")]
        public IActionResult Create()
        {
            var model = new TaiXeViewModel();
            return View(model);
        }
        [HttpPost]
        [Route("/admin/them-tai-xe/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiXeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo mã tài xế tự động
                    var lastTaiXe = _context.TaiXes.OrderByDescending(t => t.TaiXeID).FirstOrDefault();
                    int newIdNumber = 1; // Khởi tạo số thứ tự mặc định là 1 nếu không có tài xế nào trong DB

                    if (lastTaiXe != null)
                    {
                        // Bỏ 'TX' và lấy phần số, tăng giá trị lên 1
                        string lastId = lastTaiXe.TaiXeID.Substring(2); // Bỏ 'TX' và lấy phần số
                        newIdNumber = int.Parse(lastId) + 1;
                    }

                    // Định dạng mã tài xế với tiền tố 'TX' và số thứ tự có 3 chữ số
                    string newTaiXeID = "TX" + newIdNumber.ToString("D3");

                    // Tạo đối tượng TaiXe mới
                    var taiXe = new TaiXe
                    {
                        TaiXeID = newTaiXeID,
                        HoTen = model.HoTen,
                        SDT = model.SDT,
                        CCCD = model.CCCD,
                        BangLaiXe = model.BangLaiXe
                    };

                 
                    _context.Add(taiXe);
                    await _context.SaveChangesAsync();

                  
                    TempData["SuccessMessage"] = "Thêm tài xế thành công!";
                    ModelState.Clear(); // Xóa trạng thái Model để làm mới form
                    return View(model); // Giữ lại trang Create
                }
                catch (Exception ex)
                {
                  
                    Console.WriteLine(ex.Message);
                  
                    ModelState.AddModelError("", "Không thể thêm tài xế. Vui lòng thử lại.");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/xoa-tai-xe/{id?}")]
        public async Task<IActionResult> Delete(string id)
        {
           
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID không hợp lệ.";
                return NotFound();
            }

            // Tìm tài xế dựa vào ID
            var taiXe = await _context.TaiXes.FindAsync(id);
            if (taiXe == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài xế.";
                return NotFound();
            }

            try
            {

                _context.TaiXes.Remove(taiXe);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa tài xế thành công!";
            }
            catch (DbUpdateException dbEx)
            {

                Console.WriteLine(dbEx.Message);
                TempData["ErrorMessage"] = "Không thể xóa tài xế do có liên quan đến dữ liệu khác.";
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi. Vui lòng thử lại.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
