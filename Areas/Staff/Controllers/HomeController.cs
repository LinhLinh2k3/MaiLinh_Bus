using Microsoft.AspNetCore.Mvc;

namespace NhaXeMaiLinh.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class HomeController : Controller
    {
        [Route("/staff/dashboard")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
