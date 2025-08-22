using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;
using System.Globalization;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AnalysisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnalysisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang tổng quan phân tích
        [HttpGet]
        [Route("/admin/analysis")]
        public async Task<IActionResult> Index(string startDate, string endDate, string routeId)
        {
            // Mặc định: 15 ngày trước và hôm nay
            DateTime start = DateTime.Today.AddDays(-15);
            DateTime end = DateTime.Today;

            // Nếu người dùng nhập ngày tùy chỉnh
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime.TryParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out start);
                DateTime.TryParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out end);
            }

            // **1. Doanh thu theo ngày hiện tại**
            var todayRevenue = await _context.HoaDons
                .Where(hd => hd.TrangThaiThanhToan == "đã thanh toán" && hd.NgayLap.Date == DateTime.Today)
                .SumAsync(hd => (decimal?)hd.TongTien) ?? 0;

            // **2. Doanh thu theo ngày trong khoảng thời gian**
            var revenueByDay = await _context.HoaDons
                .Where(hd => hd.TrangThaiThanhToan == "đã thanh toán" && hd.NgayLap.Date >= start && hd.NgayLap.Date <= end)
                .GroupBy(hd => hd.NgayLap.Date)
                .Select(group => new
                {
                    Ngay = group.Key,
                    DoanhThu = group.Sum(hd => hd.TongTien)
                })
                .OrderBy(x => x.Ngay)
                .ToListAsync();

            // **3. Doanh thu theo 12 tháng của năm hiện tại**
            var currentYear = DateTime.Today.Year;
            var revenueByMonth = await _context.HoaDons
                .Where(hd => hd.TrangThaiThanhToan == "đã thanh toán" && hd.NgayLap.Year == currentYear)
                .GroupBy(hd => hd.NgayLap.Month)
                .Select(group => new
                {
                    Thang = group.Key,
                    DoanhThu = group.Sum(hd => hd.TongTien)
                })
                .OrderBy(x => x.Thang)
                .ToListAsync();

            var fullYearRevenue = Enumerable.Range(1, 12)
                .Select(month => new
                {
                    Thang = month,
                    DoanhThu = revenueByMonth.FirstOrDefault(x => x.Thang == month)?.DoanhThu ?? 0
                })
                .ToList();

            // **4. Doanh thu theo tuyến đường**
            //var revenueByRoute = await _context.HoaDons
            //    .Where(hd => hd.TrangThaiThanhToan == "đã thanh toán" && hd.NgayLap.Year == currentYear)
            //    .Join(_context.VeXes, hd => hd.VeID, vx => vx.VeID, (hd, vx) => new { hd, vx })
            //    .Join(_context.LichTrinhs, hv => hv.vx.LichTrinhID, lt => lt.LichTrinhId, (hv, lt) => new { hv.hd, lt })
            //    .Join(_context.TuyenDuongs, hl => hl.lt.TuyenDuongId, td => td.TuyenDuongID, (hl, td) => new { hl.hd, td })
            //    .GroupBy(x => x.td.TenTuyenDuong)
            //    .Select(group => new
            //    {
            //        TuyenDuong = group.Key,
            //        DoanhThu = group.Sum(x => x.hd.TongTien)
            //    })
            //    .OrderByDescending(x => x.DoanhThu)
            //    .ToListAsync();
            var routeList = await _context.TuyenDuongs.Select(td => td.TenTuyenDuong)
                                 .ToListAsync();

          



            // **4. Doanh thu theo tuyến đường**
            var revenueByRouteQuery = _context.HoaDons
                .Where(hd => hd.TrangThaiThanhToan == "đã thanh toán" && hd.NgayLap.Year == currentYear)
                .Join(_context.VeXes, hd => hd.VeID, vx => vx.VeID, (hd, vx) => new { hd, vx })
                .Join(_context.LichTrinhs, hv => hv.vx.LichTrinhID, lt => lt.LichTrinhId, (hv, lt) => new { hv.hd, lt })
                .Join(_context.TuyenDuongs, hl => hl.lt.TuyenDuongId, td => td.TuyenDuongID, (hl, td) => new { hl.hd, td });

            if (!string.IsNullOrEmpty(routeId))
            {
                revenueByRouteQuery = revenueByRouteQuery.Where(x => x.td.TuyenDuongID == routeId);
            }

            var revenueByRoute = await revenueByRouteQuery
                .GroupBy(x => x.td.TenTuyenDuong)
                .Select(group => new
                {
                    TuyenDuong = group.Key,
                    DoanhThu = group.Sum(x => x.hd.TongTien)
                })
                .OrderByDescending(x => x.DoanhThu)
                .ToListAsync();


            // Truyền dữ liệu vào ViewData
            ViewData["TodayRevenue"] = todayRevenue;          // Doanh thu ngày hiện tại
            ViewData["RevenueByDay"] = revenueByDay;          // Doanh thu theo ngày
            ViewData["RevenueByMonth"] = fullYearRevenue;     // Doanh thu 12 tháng
            ViewData["RevenueByRoute"] = revenueByRoute;      // Doanh thu theo tuyến đường
            ViewData["StartDate"] = start.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = end.ToString("yyyy-MM-dd");
            ViewData["RouteList"] = routeList; // Danh sách tuyến đường
            return View();


        }

    }
}
