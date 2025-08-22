using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using NhaXeMaiLinh.Models.Mail;
using NhaXeMaiLinh.Services.Email;
using NhaXeMaiLinh.ViewModels;
using System;
using System.Security.Claims;


namespace NhaXeMaiLinh.Controllers
{
    [Authorize] // Ep dang nhap cho toan bo controller
    public class ThanhToanController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public ThanhToanController(ApplicationDbContext context, IEmailSender emailSender, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        [Route("/thanh-toan")]
        public IActionResult Index()
        {
            var lichtrinhId = TempData["LichTrinhId"].ToString();
            var lichTrinh = _context.LichTrinhs
                .Include(l => l.TuyenDuong)
                .FirstOrDefault(l => l.LichTrinhId == lichtrinhId);
            
            ViewData["TenTuyenDuong"] = lichTrinh.TuyenDuong.TenTuyenDuong;
            ViewData["ThoiGian"] = lichTrinh.GioKhoiHanh + " - " + lichTrinh.GioDen;
            ViewData["SoLuongGhe"] = TempData["SoLuongGhe"];
            ViewData["GiaTien"] = TempData["GiaTien"];

            ThongTinThanhToan thongTinThanhToan = new ThongTinThanhToan();
            string KhachhangId = _context.KhachHangs.First(kh => kh.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).KhachHangID;
            thongTinThanhToan.LichTrinh = _context.LichTrinhs.First(v => v.LichTrinhId == lichtrinhId);
            thongTinThanhToan.KhachHang = _context.KhachHangs.First(kh => kh.KhachHangID == KhachhangId);

            TempData.Keep();
            return View(thongTinThanhToan);
        }

        [HttpGet]
        [Route("/thong-tin/lich-trinh/{id?}")]
        public IActionResult LayThongTinLuotDi(string id)  // Đổi kiểu lichTrinhId thành string
        {
            if(!string.IsNullOrEmpty(id))
            {
                var lichTrinh = _context.LichTrinhs
                    .Include(l => l.TuyenDuong)
                    .FirstOrDefault(l => l.LichTrinhId == id);
                return Json(lichTrinh);
            }
            return BadRequest();
        }


        [Route("/thanh-toan/thanh-cong")]
        public async Task<IActionResult> ThanhCong()
        {
            if (TempData["orderId"] != null)
            {
                // Lấy mã đơn hàng và mã giao dịch
                var orderId = TempData["orderId"].ToString();
                var transId = TempData["transId"].ToString();
                var khachhangId = _context.KhachHangs.First(k => k.AppUserId == _userManager.GetUserId(User)).KhachHangID;
                var lichtrinhId = TempData["LichTrinhId"].ToString();
                var giaTien = TempData["GiaTien"].ToString();
                var orderInfor = TempData["orderInfor"].ToString();
                var PTTT = TempData["PTTT"].ToString();
                var dsGhe = new List<string>(TempData["dsGhe"].ToString().Split(","));
                var dsTenGhe = TempData["dsTenGhe"].ToString();

                // ktra vé xe (có oderId) đã tồn tại chưa
                var ve = _context.VeXes.FirstOrDefault(v => v.VeID == orderId);
                if (ve == null)
                {
                    var createdTime = DateTime.Now;
                    var vexe = new VeXe()
                    {
                        VeID = orderId,
                        KhachHangID = khachhangId,
                        LichTrinhID = lichtrinhId,
                        KhuyenMaiID = "NONE",
                        TongGiaVe = Convert.ToDecimal(giaTien), //ép giá tiền từ string sang decimal
                        KhachHang = _context.KhachHangs.First(k => k.KhachHangID == khachhangId), // Để xử lý ở dưới
                        LichTrinh = _context.LichTrinhs.Include(l => l.TuyenDuong).First(l => l.LichTrinhId == lichtrinhId)
                    };
                    _context.Add(vexe);

                    // Cập nhật ghế khi đã chốt thành công
                    foreach (var gheId in dsGhe)
                    {
                        var ghe = _context.Ghes.FirstOrDefault(g => g.GheID == Convert.ToInt32(gheId));
                        if (ghe != null)
                        {
                            ghe.TrangThai = "mua";
                            _context.Update(ghe);

                            var chitietvedat = new ChiTietVeDat()
                            {
                                ChiTietVeID = Guid.NewGuid().ToString(),
                                VeID = orderId,
                                SoGhe = ghe.GheID,
                                XeID = ghe.XeID,
                                GiaGhe = (decimal)_context.LichTrinhs.First(l => l.LichTrinhId == lichtrinhId).GiaVe,
                                TinhTrangGhe = "đã mua",
                                NgayDat = createdTime
                            };
                            _context.Add(chitietvedat);
                        }
                    }

                    // Lưu lại lịch sử giao dịch
                    var lsgd = new LichSuGiaoDich()
                    {
                        GiaoDichID = Guid.NewGuid().ToString(),
                        VeID = orderId,
                        LoaiGiaoDich = "online",
                        ChiTiet = orderInfor,
                        NgayGiaoDich = createdTime,
                        NhanVienID = null,
                        TrangThaiGiaoDich = "đã thanh toán"
                    };
                    _context.Add(lsgd);

                    // Xuất hóa đơn
                    var hoadon = new HoaDon()
                    {
                        VeID = orderId,
                        NhanVienID = null,
                        NgayLap = createdTime,
                        TongTien = Convert.ToDecimal(giaTien),
                        PhuongThucThanhToan = PTTT,
                        TrangThaiThanhToan = "đã thanh toán"
                    };
                    _context.Add(hoadon);
                    await _context.SaveChangesAsync();

                    // Gửi email thông báo cho khách hàng
                    var maildata = new MailData
                    {
                        TenKhachHang = vexe.KhachHang.HoTen!,
                        MaVe = vexe.VeID!,
                        MaGiaoDich = transId!,
                        TenTuyenDuong = vexe.LichTrinh.TuyenDuong.TenTuyenDuong,
                        NgayKhoiHanh = vexe.LichTrinh.NgayKhoiHanh.ToString("dd/MM/yyyy"),
                        GioKhoiHanh = vexe.LichTrinh.GioKhoiHanh.ToString("HH:mm"),
                        GheChon = dsTenGhe!,
                        TongTien = vexe.TongGiaVe.ToString(),
                        PhuongThucThanhToan = PTTT!,
                        LinkChiTiet = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/tra-cuu/search?keyword={vexe.VeID}"
                    };

                    string htmlMsg = LoadHtmlTemplate("VeXe_Email.html", maildata);

                    await _emailSender.SendEmail(vexe.KhachHang.Email!, "Đặt vé thành công", htmlMsg);

                    // Hiển thị thông báo thành công và chuyển hướng
                    TempData["transId"] = transId; // Lưu mã giao dịch vào TempData để hiển thị
                    return View();
                }
                else
                {
                    return BadRequest("Thanh toán thất bại");
                }
            }

            // Nếu không tìm thấy đơn hàng hoặc có lỗi, chuyển hướng đến trang lỗi
            return RedirectToAction("ThatBai");
        }

        [Route("/thanh-toan/that-bai")]
        public IActionResult ThatBai()
        {
            return View();
        }

        [Route("/thanh-toan/loi")]
        public IActionResult Loi()
        {
            return View();
        }

        [Route("/thanh-toan")]
        public IActionResult ThanhToan()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessPayment(string paymentMethod, int amount, string orderId, string orderInfo)
        {
            switch (paymentMethod)
            {
                case "vnpay":
                    TempData["PTTT"] = "VNPay";
                    TempData["orderInfor"] = orderInfo;
                    return RedirectToAction("Payment", "Vnpay", new { area = "VNPay", amount, orderId, orderInfo });

                case "momo":
                    TempData["PTTT"] = "MoMo";
                    TempData["orderInfor"] = orderInfo;
                    return RedirectToAction("Payment", "Momo", new { area = "MoMo", amount, orderId, orderInfo });

                case "zalopay":
                    TempData["orderInfor"] = orderInfo;
                    TempData["PTTT"] = "ZaloPay";
                    return RedirectToAction("Payment", "Zalopay", new { area = "ZaloPay", amount, orderId, orderInfo });
            }
            TempData["orderInfor"] = orderInfo;
            TempData.Keep();
            return RedirectToAction(nameof(ThanhToan));
        }

        // Load mẫu email với dữ liệu
        public string LoadHtmlTemplate(string templateFileName, MailData data)
        {
            string filePath = Path.Combine(_env.WebRootPath, "mail", templateFileName);

            string htmlTemplate = System.IO.File.ReadAllText(filePath);

            // Simple string replacement for this example (can use a templating engine like Razor or Mustache)
            htmlTemplate = htmlTemplate.Replace("{{TenKhachHang}}", data.TenKhachHang);
            htmlTemplate = htmlTemplate.Replace("{{MaVe}}", data.MaVe);
            htmlTemplate = htmlTemplate.Replace("{{MaGiaoDich}}", data.MaGiaoDich);
            htmlTemplate = htmlTemplate.Replace("{{TenTuyenDuong}}", data.TenTuyenDuong);
            htmlTemplate = htmlTemplate.Replace("{{NgayKhoiHanh}}", data.NgayKhoiHanh);
            htmlTemplate = htmlTemplate.Replace("{{GioKhoiHanh}}", data.GioKhoiHanh);
            htmlTemplate = htmlTemplate.Replace("{{GheChon}}", data.GheChon);
            htmlTemplate = htmlTemplate.Replace("{{TongTien}}", data.TongTien);
            htmlTemplate = htmlTemplate.Replace("{{PhuongThucThanhToan}}", data.PhuongThucThanhToan);
            htmlTemplate = htmlTemplate.Replace("{{LinkChiTiet}}", data.LinkChiTiet);

            return htmlTemplate;
        }
    }
}
