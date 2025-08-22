using Microsoft.AspNetCore.Mvc;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text;

namespace NhaXeMaiLinh.Controllers
{
    public class HanhTrinhController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HanhTrinhController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action hiển thị ban đầu
        [Route("/lich-trinh")]
        public async Task<IActionResult> Index()
        {
            var lichTrinh = await _context.LichTrinhs
                .Include(l => l.Xe)
                .ThenInclude(x => x.LoaiXe)
                .Include(l => l.TuyenDuong)
                .Include(l => l.VeXe)
                .ToListAsync();

            return View(lichTrinh);
            //return View();
        }

        //test trc phần chọn xe chọn đúng nhưng chọn giờ đi hơi sai
        public async Task<IActionResult> Filter(string diemDi, string diemDen, string loaiXe, string gioKhoiHanh, DateOnly? ngayKhoiHanh)
        {
            var lichTrinh = _context.LichTrinhs
                .Include(l => l.Xe)
                .ThenInclude(x => x.LoaiXe)
                .Include(l => l.TuyenDuong)
                //.Where(l => l.NgayKhoiHanh >= DateOnly.FromDateTime(DateTime.Now) && l.NgayDen >= DateOnly.FromDateTime(DateTime.Now))
                .AsQueryable();


           // Lọc theo điểm đi(không phân biệt hoa thường, tìm kiếm liên quan)
            if (!string.IsNullOrEmpty(diemDi))
            {
                lichTrinh = lichTrinh.Where(l => EF.Functions.Like(l.TuyenDuong.DiemDi.ToLower(), $"%{diemDi.ToLower()}%"));
            }

            // Lọc theo điểm đến
            if (!string.IsNullOrEmpty(diemDen))
            {
                lichTrinh = lichTrinh.Where(l => EF.Functions.Like(l.TuyenDuong.DiemDen.ToLower(), $"%{diemDen.ToLower()}%"));
            }


            //string diemDiAscii = string.IsNullOrEmpty(diemDi) ? null : ConvertToAscii(diemDi);
            //string diemDenAscii = string.IsNullOrEmpty(diemDen) ? null : ConvertToAscii(diemDen);

            //// Bước 3: Lọc theo điểm đi sau khi đã chuyển sang ASCII
            //if (!string.IsNullOrEmpty(diemDiAscii))
            //{
            //    lichTrinh = lichTrinh.Where(l => EF.Functions.Like(l.TuyenDuong.DiemDi.ToLower(), $"%{diemDiAscii.ToLower()}%"));
            //}

            //// Bước 4: Lọc theo điểm đến sau khi đã chuyển sang ASCII
            //if (!string.IsNullOrEmpty(diemDenAscii))
            //{
            //    lichTrinh = lichTrinh.Where(l => EF.Functions.Like(l.TuyenDuong.DiemDen.ToLower(), $"%{diemDenAscii.ToLower()}%"));
            //}

            // Lọc theo loại xe
            if (!string.IsNullOrEmpty(loaiXe) && loaiXe != "Chọn loại xe")
            {
                lichTrinh = lichTrinh.Where(l => l.Xe.LoaiXe.TenLoaiXe.ToLower().Contains(loaiXe.ToLower()));
            }


			// Lọc theo ngày khởi hành nếu có
			if (ngayKhoiHanh.HasValue)
			{
				// So sánh với ngày khởi hành trong cơ sở dữ liệu
				lichTrinh = lichTrinh.Where(l => l.NgayKhoiHanh == ngayKhoiHanh.Value);
			}

			// Lọc theo giờ khởi hành (kiểm tra thời gian gần với giờ người dùng chọn)
			if (!string.IsNullOrEmpty(gioKhoiHanh))
            {
                string[] gioKhoiHanhRange = gioKhoiHanh.Split('-');
                if (gioKhoiHanhRange.Length == 2)
                {
                    TimeOnly gioBatDau = TimeOnly.Parse(gioKhoiHanhRange[0]);
                    TimeOnly gioKetThuc = TimeOnly.Parse(gioKhoiHanhRange[1]);

                    // Tạo DateTime từ TimeOnly
                    DateTime gioBatDauDateTime = DateTime.Today.Add(gioBatDau.ToTimeSpan());
                    DateTime gioKetThucDateTime = DateTime.Today.Add(gioKetThuc.ToTimeSpan());

                    // Thời gian dung sai ±30 phút
                    TimeSpan tolerance = TimeSpan.FromMinutes(30);

                    // Thực hiện so sánh giờ khởi hành
                    lichTrinh = lichTrinh.Where(l =>
                        (l.GioKhoiHanh >= gioBatDau && l.GioKhoiHanh <= gioKetThuc) ||
                        (l.GioKhoiHanh >= TimeOnly.FromDateTime(gioBatDauDateTime - tolerance) &&
                         l.GioKhoiHanh <= TimeOnly.FromDateTime(gioBatDauDateTime + tolerance)) ||
                        (l.GioKhoiHanh >= TimeOnly.FromDateTime(gioKetThucDateTime - tolerance) &&
                         l.GioKhoiHanh <= TimeOnly.FromDateTime(gioKetThucDateTime + tolerance))
                    );
                }
            }
			
			var result = await lichTrinh.ToListAsync();

            if (result == null || !result.Any())
            {
                // Trả về thông báo nếu không có kết quả
                return PartialView("KetQuaTimKiem");
            }

            // Trả về PartialView hiển thị bảng kết quả
            return PartialView("_LichTrinhTable", result);
        }

        public IActionResult NoResultMessage()
        {
            // Trả về PartialView chứa thông báo "Không có kết quả được tìm thấy"
            return PartialView("KetQuaTimKiem");
        }

        [HttpPost]
        public IActionResult SaveSeats([FromBody] SeatRequest data)
        {
            if (data != null)
            {
                var seats = data.seats;
                var lichTrinhId = data.lichTrinhId;
                var soLuongGhe = seats.Count;
                var dsGheStr = string.Join(",", data.IDseats);
                var dsTenGheStr = string.Join(", ", seats);

                // Lưu thông tin ghế và lịch trình vào TempData
                TempData["dsTenGhe"] = dsTenGheStr;
                TempData["dsGhe"] = dsGheStr;
                TempData["LichTrinhId"] = lichTrinhId;
                TempData["GiaTien"] = data.GiaTien;
                TempData["SoLuongGhe"] = soLuongGhe;
                TempData.Keep();

                // Trả về phản hồi thành công cho AJAX
                return Ok();
            }
            else
            {
                // Trả về lỗi nếu dữ liệu không hợp lệ
                return BadRequest("Dữ liệu không hợp lệ.");
            }
        }
        [HttpGet]
        [Route("/thong-tin/ghe")]
        public async Task<IActionResult> GetSeats(string id)
        {
            var dslichtrinh = _context.LichTrinhs.Include(l => l.Xe).FirstOrDefault(l => l.LichTrinhId == id && l.Xe.IsDeleted == true);
            if (dslichtrinh == null) return Json(null);

            var dsGhe = _context.Ghes.Include(g => g.Xe)
                .Where(g => g.XeID == dslichtrinh.XeId)
                .OrderBy(g => g.TenGhe).ToList();
            return Json(dsGhe);
        }

        public static string ConvertToAscii(string input)
        {
            var normalizedString = input.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

    }
    public class SeatRequest
    {
        public string lichTrinhId { get; set; }
        public List<string> seats { get; set; }
        public List<int> IDseats { get; set; }
        public int GiaTien { get; set; }
    }
   
}
