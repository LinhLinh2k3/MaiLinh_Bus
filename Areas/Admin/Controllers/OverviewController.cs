using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Admin.ViewModels;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OverviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OverviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        //[Route("/admin/dasboard")]
        [Route("/admin/tong-quan")]
        public async Task<IActionResult> Index()
        {
            // 1. Dữ liệu từ module Khuyến Mãi
            // Tổng khuyến mãi trong hệ thống
            var totalPromotions = await _context.KhuyenMais.CountAsync();
            var activePromotions = await _context.KhuyenMais.CountAsync(km => km.TrangThaiThanhToan == "1");

            // 2. Dữ liệu từ module Nhân Viên
            var totalEmployees = await _context.NhanViens.CountAsync();
            // var activeEmployees = await _context.NhanViens.CountAsync();
            // Số lượng nhân viên đang hoạt động
            var activeEmployees = await _context.NhanViens.CountAsync(nv => nv.AppUser.isEnabled == true);

            // 3. Dữ liệu từ module Lịch Trình
            var totalSchedules = await _context.LichTrinhs.CountAsync();
          //  var schedulesToday = await _context.LichTrinhs.CountAsync(lt => lt.NgayKhoiHanh == DateTime.Today);
            var schedulesToday = await _context.LichTrinhs.CountAsync(lt => lt.NgayKhoiHanh == DateOnly.FromDateTime(DateTime.Today));

            // 4. Dữ liệu từ module Xe
            // Tổng số xe trong hệ thống
            var totalVehicles = await _context.Xes.CountAsync();

            // Số lượng xe đang hoạt động (IsDeleted == true)
            var activeVehicles = await _context.Xes.CountAsync(x => x.IsDeleted == true);


            // 5. Dữ liệu từ module Tài Xế
            var totalDrivers = await _context.TaiXes.CountAsync();
            var availableDrivers = await _context.TaiXes.CountAsync();

            // Tạo ViewModel tổng quan
            var model = new OverviewViewModel
            {
                TotalPromotions = totalPromotions,
                ActivePromotions = activePromotions,

                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,

                TotalSchedules = totalSchedules,
                SchedulesToday = schedulesToday,

                TotalVehicles = totalVehicles,
                ActiveVehicles = activeVehicles,

                TotalDrivers = totalDrivers,
                AvailableDrivers = availableDrivers,
            };

            return View(model);
        }
    }
}
