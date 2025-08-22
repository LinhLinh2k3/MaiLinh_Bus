using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Staff.Models;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using System.Security.Cryptography;

namespace NhaXeMaiLinh.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public KhachHangController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/staff/khach-hang/quan-ly")]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var dskh = _context.KhachHangs.Include(k => k.AppUser)
                .Where(k => k.AppUser.isEnabled == true) // chỉ những tài khoản đang hoạt động!
                .AsQueryable();

            int total = dskh.Count();

            var dskhPage = dskh.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewmodel = new SearchResult
            {
                KhachHangs = dskhPage,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = total
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_dskh", viewmodel);
            }

            return View(viewmodel);
        }

        [Route("/staff/khach-hang/tim-kiem")]
        [HttpGet]
        public IActionResult Search(int searchType, string searchQuery, int page = 1, int pageSize = 10)
        {
            var khachHangs = _context.KhachHangs.Include(k => k.AppUser)
                .Where(k => k.AppUser.isEnabled == true) // chỉ những tài khoản đang hoạt động!
                .AsQueryable();

            // Filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                switch (searchType)
                {
                    case 1: // Tìm kiếm theo Tên khách hàng
                        khachHangs = khachHangs.Where(k => k.HoTen.Contains(searchQuery));
                        break;
                    case 2: // Tìm kiếm theo Số điện thoại
                        khachHangs = khachHangs.Where(k => k.SDT.Contains(searchQuery));
                        break;
                    case 3: // Tìm kiếm theo Mã khách hàng
                        khachHangs = khachHangs.Where(c => c.KhachHangID.Contains(searchQuery));
                        break;
                    default:
                        break;
                }
            }

            // Total count for pagination
            int totalRecords = khachHangs.Count();

            // Apply pagination
            var pagedCustomers = khachHangs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create a view model for results and pagination
            var viewModel = new SearchResult
            {
                KhachHangs = pagedCustomers,
                PageNumber = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            // Return a partial view for AJAX updates
            return PartialView("_dskh", viewModel);
        }

        [Route("/staff/khach-hang/chi-tiet/{id?}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.AppUser)
                .FirstOrDefaultAsync(m => m.KhachHangID == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        [Route("/staff/khach-hang/them")]
        public IActionResult Create()
        {
            // Hạng thành viên của khách hàng
            var dsHangThanhVien = new List<SelectListItem>()
            {
                new() { Value = "Member", Text = "Thành viên" },
                new() { Value = "Classic", Text = "Phổ thông" },
                new() { Value = "VIP", Text = "VIP" },
            };
            ViewData["DSHangThanhVien"] = new SelectList(dsHangThanhVien, "Value", "Text");
            return View();
        }

        [Route("/staff/khach-hang/them")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,SDT,Email,CCCD,DiaChi,HangThanhVien")] ThemKhachHang ThemKhachHang)
        {
            if (ModelState.IsValid)
            {
                // Giải thích:
                // Vì thông tin tập hợp bên bảng "KhachHang" hơn là bảng AppUser (IdentityUser)
                // nên theo tiến trình: nạp AppUser trước, lấy Id của AppUser để nạp vào KhachHang
                // Lưu ý: thông tin giữa bảng AppUser với bảng KhachHang/NhanVien/TaiXe có sự trùng lặp (bất hợp lý)
                var user = new AppUser();
                await _userManager.SetEmailAsync(user, ThemKhachHang.Email);
                await _userManager.SetPhoneNumberAsync(user, ThemKhachHang.SDT);
                await _userManager.SetUserNameAsync(user, ThemKhachHang.Email.Split('@')[0]);
                user.EmailConfirmed = true; // Bỏ qua xác thực (nếu có)
                user.PhoneNumberConfirmed = true; // Bỏ qua xác thực (nếu có)
                user.Name = ThemKhachHang.HoTen;
                user.Address = ThemKhachHang.DiaChi;
                user.isEnabled = true; // Mặc định

                var password = GeneratePassword(); // Mật khẩu ngẫu nhiên, vui lòng yêu cầu khách hàng reset mật khẩu

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Cấp quyền
                    await _userManager.AddToRoleAsync(user, "KhachHang");

                    // Điền thông tin khách hàng
                    var khachHang = new KhachHang()
                    {
                        HoTen = ThemKhachHang.HoTen,
                        SDT = ThemKhachHang.SDT,
                        Email = ThemKhachHang.Email,
                        CCCD = ThemKhachHang.CCCD,
                        DiaChi = ThemKhachHang.DiaChi,
                        HangThanhVien = ThemKhachHang.HangThanhVien,
                        AppUserId = user.Id
                    };

                    _context.Add(khachHang);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                // lỗi
                return View(ThemKhachHang);
            }

            // Hạng thành viên của khách hàng
            var dsHangThanhVien = new List<SelectListItem>()
            {
                new() { Value = "Member", Text = "Thành viên" },
                new() { Value = "Classic", Text = "Phổ thông" },
                new() { Value = "VIP", Text = "VIP" },
            };
            ViewData["DSHangThanhVien"] = new SelectList(dsHangThanhVien, "Value", "Text", ThemKhachHang.HangThanhVien);

            // lỗi
            return View(ThemKhachHang);
        }

        [Route("/staff/khach-hang/chinh-sua/{id?}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            // Hạng thành viên của khách hàng
            var dsHangThanhVien = new List<SelectListItem>()
            {
                new() { Value = "Member", Text = "Thành viên" },
                new() { Value = "Classic", Text = "Phổ thông" },
                new() { Value = "VIP", Text = "VIP" },
            };
            ViewData["DSHangThanhVien"] = new SelectList(dsHangThanhVien, "Value", "Text", khachHang.HangThanhVien);
            return View(khachHang);
        }

        [Route("/staff/khach-hang/chinh-sua/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("KhachHangID,HoTen,SDT,Email,CCCD,DiaChi,HangThanhVien")] ChinhSuaKhachHang khachHang)
        {
            if (id != khachHang.KhachHangID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var kh = _context.KhachHangs.First(n => n.KhachHangID == khachHang.KhachHangID);
                    kh.HoTen = khachHang.HoTen;
                    kh.SDT = khachHang.SDT;
                    kh.Email = khachHang.Email;
                    kh.CCCD = khachHang.CCCD;
                    kh.DiaChi = khachHang.DiaChi;
                    kh.HangThanhVien = khachHang.HangThanhVien;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.KhachHangID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Hạng thành viên của khách hàng
            var dsHangThanhVien = new List<SelectListItem>()
            {
                new() { Value = "Member", Text = "Thành viên" },
                new() { Value = "Classic", Text = "Phổ thông" },
                new() { Value = "VIP", Text = "VIP" },
            };
            ViewData["DSHangThanhVien"] = new SelectList(dsHangThanhVien, "Value", "Text", khachHang.HangThanhVien);
            return View(khachHang);
        }

        [Route("/staff/khach-hang/xoa/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            // Giải thích:
            // Vì trước đó mình theo tiến trình lúc tạo là AppUser -> KhachHang,
            // Nên lúc xóa thì mình làm ngược lại tiến trình này
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                var userId = khachHang.AppUserId;

                // Chuyển tài khoản này về trạng thái không hoạt động! (Chính sách)
                var user = await _userManager.FindByIdAsync(userId!);
                if (user != null)
                {
                    user.isEnabled = false;
                    await _userManager.UpdateAsync(user);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.KhachHangID == id);
        }

        private static string GeneratePassword(int length = 14)
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()";

            string allChars = uppercase + lowercase + digits + specialChars;

            var randomNumber = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = allChars[randomNumber[i] % allChars.Length];
            }

            // Ensure the password contains at least one character from each category
            if (!chars.Any(char.IsUpper) || !chars.Any(char.IsLower) || !chars.Any(char.IsDigit) || !chars.Any(ch => specialChars.Contains(ch)))
            {
                // Get random positions for each category
                var rand = new Random();
                int posUpper = rand.Next(0, length);
                int posLower, posDigit, posSpecial;
                do { posLower = rand.Next(0, length); } while (posLower == posUpper);
                do { posDigit = rand.Next(0, length); } while (posDigit == posUpper || posDigit == posLower);
                do { posSpecial = rand.Next(0, length); } while (posSpecial == posUpper || posSpecial == posLower || posSpecial == posDigit);

                chars[posUpper] = uppercase[randomNumber[posUpper] % uppercase.Length];
                chars[posLower] = lowercase[randomNumber[posLower] % lowercase.Length];
                chars[posDigit] = digits[randomNumber[posDigit] % digits.Length];
                chars[posSpecial] = specialChars[randomNumber[posSpecial] % specialChars.Length];
            }

            return new string(chars.OrderBy(x => Guid.NewGuid()).ToArray());
        }
    }
}
