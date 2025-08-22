using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models;

namespace NhaXeMaiLinh.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("/")]
        public IActionResult TrangChu()
        {
            if (User != null && User.IsInRole("Admin"))
            {
               return LocalRedirect("/admin/login");
            }
            else if (User != null && User.IsInRole("NhanVien"))
            {
                return LocalRedirect("/staff/dashboard");
            }

			return View();
        }

		[Route("/ve-chung-toi")]
        public IActionResult VeChungToi()
        {
            return View();
        }

        [Route("/lien-he")]
        public IActionResult LienHe()
        {
            return View();
        }

        public IActionResult ChinhSach()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
