using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;

namespace NhaXeMaiLinh.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class GiaoDichController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiaoDichController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/staff/giao-dich/quan-ly")]
        public async Task<IActionResult> Index()
        {
            var lsgd = _context.LichSuGiaoDichs.Include(l => l.NhanVien).Include(l => l.VeXe.KhachHang);
            return View(await lsgd.ToListAsync());
        }
    }
}