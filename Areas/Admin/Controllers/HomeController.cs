using Microsoft.AspNetCore.Mvc;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        [Route("/admin/dasboard")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/admin/login")]
        [HttpGet]
        public IActionResult Login_Admin()
        {
            return RedirectToAction("Index");
        }
        
    }
}
