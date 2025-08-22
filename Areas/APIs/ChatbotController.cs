using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Services;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Areas.APIs
{
    [ApiController]
    [Route("/api")]
    public class ChatbotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly Chatbot _chatbot;

        public ChatbotController(ApplicationDbContext context, Chatbot chatbot)
        {
            _context = context;
            _chatbot = chatbot;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Index([FromBody] string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return BadRequest("Message cannot be empty.");
            }

            // nếu người dùng hỏi về vé
            if (msg.Contains("vé") && msg.Contains("còn"))
            {
                int seats = _context.Ghes.Count(g => g.TrangThai == "trống");
                string prompt = $"Khách hỏi: \"{msg}\". Dữ liệu hệ thống: \"còn {seats} vé\". Hãy trả lời rõ ràng.";
                string _response = await _chatbot.GetResponseAsync(prompt);
                return Ok(_response);
            }

            // nếu người dùng hỏi về tuyến
            if ((msg.Contains("tuyến")|| msg.Contains("nơi"))&& msg.Contains("nào"))
            {
                var listTuyen = _context.TuyenDuongs
                .Select(t => $"{t.TenTuyenDuong}")
                .ToList();
                string allRoutes = "Dạ vâng, nhà xe mình hiện đang khai thác các tuyến đường sau:<br>"
                                + string.Join("<br>", listTuyen)
                                + string.Join("\n", ". Bạn vui lòng cho tôi biết thông tin ngày, tuyến bạn muốn đi");                    
                return Ok(allRoutes);
            }

            if(msg.Contains("tuyến")&& msg.Contains("ngày"))
            {
                string prompt = $@"Tin nhắn khách hàng: '{msg}'
                    Nhiệm vụ:
                    1. Chỉ xác định và trả về TÊN TUYẾN ĐƯỜNG theo định dạng: ĐiểmĐi - ĐiểmĐến.
                       - Ví dụ hợp lệ: 'Sài Gòn - Nha Trang', 'Hà Nội - Hải Phòng'.
                       - Nếu không xác định được, trả về 'Không rõ'.

                    2. Xác định và trả về NGÀY ĐI theo định dạng chuẩn: yyyy-mm-dd.
                       - Chấp nhận các dạng ngày viết tắt hoặc có chữ (ví dụ: '5/8', '2024-12-19', 'ngày mai', 'thứ sáu tuần sau').
                       - Nếu không xác định được, trả về 'Không rõ'.

                    Yêu cầu:
                    - Chỉ trả về dữ liệu dưới dạng JSON như sau:
                      {{ ""TuyenDuong"": ""ĐiểmĐi - ĐiểmĐến"", ""NgayDi"": ""yyyy-mm-dd"" }}

                    - Không viết thêm bất kỳ giải thích hoặc văn bản nào khác ngoài JSON trên.
                    ";

                var _response = await _chatbot.GetResponseAsync(prompt);
                string jsonString = _response
                .Replace("```json", "")
                .Replace("```", "")
                .Trim(); 

                using var doc = JsonDocument.Parse(jsonString);
                string tuyen = doc.RootElement.GetProperty("TuyenDuong").GetString();
                string ngayDi = doc.RootElement.GetProperty("NgayDi").GetString();

                var listLichTrinh = _context.LichTrinhs
                    .Where(l => l.NgayKhoiHanh == DateOnly.Parse(ngayDi))
                    .Select(l => new
                    {
                        l.LichTrinhId,
                        Tuyen = $"{l.TuyenDuong.DiemDi} - {l.TuyenDuong.DiemDen}",
                        l.NgayKhoiHanh
                    })
                    .AsEnumerable() // Để xử lý so sánh ở phía C#
                    .Where(l => RemoveDiacritics(l.Tuyen).Equals(
                                RemoveDiacritics(tuyen),
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();
                string allRoutes = "Dạ vâng, nhà xe mình hiện còn tuyến "+tuyen+" vào ngày "+ngayDi+":<br>"
                                + string.Join("<br>", listLichTrinh)
                                + string.Join("\n", ". Bạn vui lòng cho tôi biết thông tin thời gian cụ thể bạn muốn đi");
                return Ok(allRoutes);
            }

            var response = await _chatbot.GetResponseAsync(msg);
            return Ok(response);
        }
        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var chars = normalized.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                                               != System.Globalization.UnicodeCategory.NonSpacingMark);
            return new string(chars.ToArray()).Normalize(System.Text.NormalizationForm.FormC);
        }

    }
}
