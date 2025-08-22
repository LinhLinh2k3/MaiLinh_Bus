using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using NhaXeMaiLinh.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using NhaXeMaiLinh.Areas.Staff.Models;

namespace NhaXeMaiLinh.Controllers
{
    public class VeXeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VeXeController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [Route("/tra-cuu")]
        public IActionResult Index()
        {
            ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
            return View(new VeXeLookupViewModel());
        }

        [HttpPost]
        [Route("/tra-cuu")]
        public async Task<IActionResult> Index(VeXeLookupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate reCAPTCHA
            var isValidCaptcha = await ValidateReCaptcha(model.ReCaptchaToken);
            if (!isValidCaptcha)
            {
                ModelState.AddModelError(string.Empty, "reCAPTCHA không hợp lệ. Vui lòng thử lại");
                return View(model);
            }

            // Redirect to the search action with the ticket ID as a query parameter
            return RedirectToAction("Search", new { keyword = model.MaVe });
        }

        [Route("/tra-cuu/search")]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index");
            }

            // Retrieve ticket details based on the keyword
            var veXeDetails = await _context.VeXes
                .Include(v => v.KhachHang)
                .Include(v => v.LichTrinh)
                .ThenInclude(lt => lt.TuyenDuong)
                .Include(v => v.LichTrinh)
                .ThenInclude(lt => lt.Xe)
                .Include(v => v.ChiTietVeDats)
                .ThenInclude(ctvd => ctvd.Ghe)
                .FirstOrDefaultAsync(v => v.VeID == keyword);

            if (veXeDetails == null)
            {
                ModelState.AddModelError(string.Empty, "KHÔNG TÌM THẤY BẠN CÓ THỂ LIÊN HỆ CHÚNG TÔI ĐỂ BIẾT THÊM CHI TIẾT");
                ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
                return View("Index", new VeXeLookupViewModel { MaVe = keyword });
            }

            // Pass the details to the view
            return View("TicketDetails", veXeDetails);
        }

        private async Task<bool> ValidateReCaptcha(string token)
        {
            var secretKey = _configuration["ReCaptcha:SecretKey"];
            var client = new HttpClient();
            var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}", null);
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonData = JObject.Parse(jsonString);
            return jsonData.success == "true";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ve-xe/huy/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var veXe = await _context.VeXes.FindAsync(id);
                if (veXe != null)
                {
                    var createdTime = DateTime.Now;

                    // Lấy thông tin lịch trình (bao gồm giờ khởi hành)
                    var lichTrinh = await _context.LichTrinhs
                        .Include(lt => lt.TuyenDuong)
                        .FirstOrDefaultAsync(lt => lt.LichTrinhId == veXe.LichTrinhID);

                    if (lichTrinh == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy lịch trình!" });
                    }

                    // Lấy ngày và giờ khởi hành (giả sử gioKhoiHanh là TimeOnly)
                    var gioKhoiHanh = lichTrinh.GioKhoiHanh;
                    var ngayKhoiHanh = lichTrinh.NgayKhoiHanh;

                    // Kết hợp ngày khởi hành với giờ khởi hành
                    var ngayGioKhoiHanh = new DateTime(ngayKhoiHanh.Year, ngayKhoiHanh.Month, ngayKhoiHanh.Day, gioKhoiHanh.Hour, gioKhoiHanh.Minute, gioKhoiHanh.Second);

                    // Tính số giờ còn lại so với thời điểm hiện tại
                    var diff = ngayGioKhoiHanh - createdTime;

                    if (diff.TotalHours > 48)
                    {
                        // Trường hợp hủy vé trong thời gian trên 48 giờ trước giờ khởi hành
                        var chitietvedat = _context.ChiTietVeDats.Where(c => c.VeID == id).ToList();
                        foreach (var ctvd in chitietvedat)
                        {
                            var ghe = await _context.Ghes.FirstOrDefaultAsync(g => g.GheID == ctvd.SoGhe);
                            if (ghe != null)
                            {
                                ghe.TrangThai = "trống";
                                _context.Update(ghe);
                            }

                            ctvd.TinhTrangGhe = "đã hủy";
                            _context.Update(ctvd);
                        }

                        // Cập nhật lịch sử giao dịch hủy vé
                        var lsgd = new LichSuGiaoDich()
                        {
                            GiaoDichID = Guid.NewGuid().ToString(),
                            VeID = veXe.VeID,
                            LoaiGiaoDich = "cancel",
                            ChiTiet = "Hủy vé với mã: " + veXe.VeID,
                            NgayGiaoDich = createdTime,
                            TrangThaiGiaoDich = "chưa thanh toán"
                        };
                        _context.Add(lsgd);

                       
                        

                        // Cập nhật trạng thái vé là đã bị xóa
                        veXe.isDelete = null;
                        _context.Update(veXe);
                        // Lưu thay đổi
                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = "Hủy vé thành công!" });
                    }
                    else
                    {
                        // Trường hợp hủy vé dưới 48 giờ
                        return Json(new { success = false, message = "Không thể hủy vé dưới 48 giờ trước giờ khởi hành!" });
                    }
                }

                return Json(new { success = false, message = "Không tìm thấy vé!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }


        [Route("/ve-xe/doi-ghe/{id?}")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin vé
            var veXe = await _context.VeXes
                .Include(v => v.LichTrinh)
                    .ThenInclude(lt => lt.TuyenDuong)
                .Include(v => v.ChiTietVeDats)
                    .ThenInclude(ct => ct.Ghe)
                .FirstOrDefaultAsync(v => v.VeID == id);

            if (veXe == null)
            {
                return NotFound();
            }

            // Lấy danh sách ghế trống trong lịch trình của vé
            var gheTrong = _context.Ghes
                .Where(g => g.XeID == veXe.LichTrinh.XeId && g.TrangThai == "trống")
                .Select(g => new SelectListItem
                {
                    Value = g.GheID.ToString(),
                    Text = g.TenGhe
                }).ToList();

            ViewData["DanhSachGheTrong"] = new SelectList(gheTrong, "Value", "Text");

            var dskm = _context.KhuyenMais.Select(km => new SelectListItem
            {
                Value = km.KhuyenMaiID,
                Text = km.TenKhuyenMai
            });

            ViewData["KhuyenMaiID"] = new SelectList(dskm, "Value", "Text", "NONE");

            return View(veXe);
        }


        [Route("/ve-xe/doi-ghe/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int GheCu, int GheMoi, string LyDo)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var createdTime = DateTime.Now;

                // Lấy thông tin vé
                var veXe = await _context.VeXes
                    .Include(v => v.LichTrinh)
                    .FirstOrDefaultAsync(v => v.VeID == id);

                if (veXe == null)
                {
                    return NotFound();
                }

                // Cập nhật ghế mới
                var gheMoi = await _context.Ghes.FirstOrDefaultAsync(g => g.GheID == GheMoi);
                if (gheMoi != null)
                {
                    gheMoi.TrangThai = "mua";
                    _context.Update(gheMoi);

                    // Thêm chi tiết vé đặt mới
                    var chitietvedatMoi = new ChiTietVeDat
                    {
                        ChiTietVeID = Guid.NewGuid().ToString(),
                        VeID = veXe.VeID,
                        SoGhe = gheMoi.GheID,
                        XeID = gheMoi.XeID,
                        GiaGhe = veXe.TongGiaVe, // Giá giữ nguyên
                        TinhTrangGhe = "đã mua",
                        NgayDat = createdTime
                    };
                    _context.Add(chitietvedatMoi);
                }
                else
                {
                    // Ghế mới không hợp lệ hoặc đã được đặt!
                    return View();
                }

                // Cập nhật ghế cũ về trạng thái "trống"
                var gheCu = await _context.Ghes.FirstOrDefaultAsync(g => g.GheID == GheCu);
                if (gheCu != null)
                {
                    gheCu.TrangThai = "trống";
                    _context.Update(gheCu);

                    // Xóa chi tiết ghế cũ
                    var chitietVeCu = await _context.ChiTietVeDats.FirstOrDefaultAsync(c => c.SoGhe == GheCu && c.VeID == veXe.VeID);
                    if (chitietVeCu != null)
                    {
                        _context.Remove(chitietVeCu);
                    }
                }

                var lsgd = new LichSuGiaoDich()
                {
                    GiaoDichID = Guid.NewGuid().ToString(),
                    VeID = veXe.VeID,
                    ChiTiet = "Đổi ghế từ " + gheCu.TenGhe + " sang ghế " + gheMoi.TenGhe + "\n" + LyDo,
                    NgayGiaoDich = createdTime,
                    NhanVienID = null
                };
                _context.Add(lsgd);

                // Thêm lịch sử đổi ghế
                var lichSuDoiGhe = new LichSuDoiGhe
                {
                    ChiTietVeID = Guid.NewGuid().ToString(),
                    GheCu = GheCu,
                    GheMoi = GheMoi,
                    NgayDoi = createdTime,
                    LyDoDoi = LyDo,
                    GiaoDichID = lsgd.GiaoDichID,
                    NhanVienID = null
                };
                _context.Add(lichSuDoiGhe);

                await _context.SaveChangesAsync();

                // Đổi ghế thành công!
                return LocalRedirect("/ve-cua-ban");
            }
            catch (Exception ex)
            {
                return View();
            }

            var dskm = _context.KhuyenMais.Select(km => new SelectListItem
            {
                Value = km.KhuyenMaiID,
                Text = km.TenKhuyenMai
            });

            ViewData["KhuyenMaiID"] = new SelectList(dskm, "Value", "Text");
            return View();
        }
    }
}
