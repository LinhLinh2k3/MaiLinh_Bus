using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;


namespace NhaXeMaiLinh.Controllers
{
    public class TinTucController : Controller
    {
        private readonly ApplicationDbContext _context;

        //CONSTRUCTOR
        public TinTucController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/tin-tuc")]
        public IActionResult Index()
        {
            var dsTinTuc = _context.TinTucs.Include(tt => tt.NhanVien).ToList();
            return View(dsTinTuc);
        }

        // Get: /tin-tuc/chi-tiet/tin-giat-gan
        [Route("/tin-tuc/chi-tiet/{url?}")]
        [HttpGet]
        public IActionResult ChiTiet(string url)
        {
            var tintuc = _context.TinTucs
                .Include(tt => tt.FileTinTuc)
                .Include(tt => tt.NhanVien)
                .FirstOrDefault(t => t.Url == url);
            if (tintuc == null)
            {
                return NotFound();
            }

            return View(tintuc);
        }
    }
}