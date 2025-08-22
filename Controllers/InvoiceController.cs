using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NhaXeMaiLinh.Models.Data;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace NhaXeMaiLinh.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public InvoiceController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [Route("/tra-cuu-hoa-don")]
        public IActionResult Index()
        {
            ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
            return View(new InvoiceLookupViewModel());
        }
        [HttpPost]
        [Route("/tra-cuu-hoa-don")]
        public async Task<IActionResult> Index(InvoiceLookupViewModel model)
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

            // Redirect to the search action with the invoice code as a query parameter
            return RedirectToAction("Search", new { keyword = model.InvoiceCode });
        }

        [Route("/tra-cuu-hoa-don/search")]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index");
            }
            // Retrieve invoice details based on the keyword
            var invoiceDetails = await _context.HoaDons
                .Include(hd => hd.VeXe)
                    .ThenInclude(vx => vx.KhachHang)
                .Include(hd => hd.VeXe)
                    .ThenInclude(vx => vx.LichTrinh)
                    .ThenInclude(lt => lt.TuyenDuong)
                .Include(hd => hd.VeXe)
                    .ThenInclude(vx => vx.LichTrinh)
                    .ThenInclude(lt => lt.Xe)
                .Include(hd => hd.VeXe)
                    .ThenInclude(vx => vx.ChiTietVeDats)
                    .ThenInclude(ctvd => ctvd.Ghe)
                .FirstOrDefaultAsync(hd => hd.HoaDonID == Convert.ToInt32(keyword));

            if (invoiceDetails == null)
            {
                ModelState.AddModelError(string.Empty, "KHÔNG TÌM THẤY HÓA ĐƠN. VUI LÒNG LIÊN HỆ CHÚNG TÔI ĐỂ BIẾT THÊM CHI TIẾT");
                ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
                return View("Index", new InvoiceLookupViewModel { InvoiceCode = keyword });
            }

            return View("InvoiceDetails", invoiceDetails);
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

    }
}
