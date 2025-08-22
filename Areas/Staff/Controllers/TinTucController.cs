using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Staff.Models;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using NhaXeMaiLinh.Services;
using System.Text;


namespace NhaXeMaiLinh.Area.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
    public class TinTucController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly ApplicationDbContext _context;
        private readonly FileManager _fileManager;
        private readonly UserManager<AppUser> _userManager;

        public TinTucController(ApplicationDbContext context, IWebHostEnvironment webHost, FileManager fileManager, UserManager<AppUser> userManager)
        {
            _context = context;
            _webHost = webHost;
            _fileManager = fileManager;
            _userManager = userManager;
        }

        [Route("/staff/tin-tuc/quan-ly")]
        public IActionResult Index(int trang = 1)
        {
            int pageSize = 5;
            var dsTinTuc = _context.TinTucs
                .Include(tt => tt.NhanVien)
                .Include(tt => tt.FileTinTuc)
                .OrderBy(tt => tt.ThoiGian)
                .Reverse()
                .Skip((trang - 1) * pageSize)
                .Take(pageSize).ToList();

            var soTinTuc = dsTinTuc.Count;
            var soTrang = (int)Math.Ceiling(soTinTuc / (double)pageSize);

            ViewBag.CurrentPage = trang;
            ViewBag.TotalPages = soTrang;

            return View(dsTinTuc);
        }

        [Route("/staff/tin-tuc/them")]
        [HttpGet]
        public IActionResult Them()
        {
            return View();
        }

        [Route("/staff/tin-tuc/chinh-sua/{id?}")]
        [HttpGet]
        public IActionResult ChinhSua(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tintuc = _context.TinTucs.Include(tt => tt.NhanVien).Include(tt => tt.FileTinTuc)
                .FirstOrDefault(t => t.TinTucId == id);

            if (tintuc == null)
            {
                return NotFound();
            }

            return View(tintuc);
        }

        [Route("/staff/tin-tuc/xem/{url?}")]
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

        private static string GenerateUrl(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormKD);
            string ascii = new(normalized.Where(c => c < 128).ToArray()); // Chuyển "Unicode" sang "ASCII"
            string r = ascii.Replace("?", string.Empty);
            string lower = r.ToLower().Replace(' ', '-'); // Thay thế khoảng trống thành '-' và chuyển thành chữ thường cho các kỹ tự
            string r1 = lower.Replace('đ', 'd');
            string result = r1 + "-" + DateTime.Now.ToString("yyyyMMdd"); // thêm thời gian
            return result;
        }

        [HttpPost]
        [Route("/staff/tin-tuc/them")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them([Bind("TieuDe,NoiDung")] ThemTinTuc ThemTinTuc, List<IFormFile> files, IFormFile AnhBia)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin nhân viên (đang đăng nhập hiện tại, người mà tương tác)
                var user = await _userManager.GetUserAsync(User);
                var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.AppUserId == user.Id);
                if (nhanvien == null)
                {
                    // Liên hệ admin
                    return NotFound();
                }

                // Tạo object "TinTuc" để chứa thông tin và nạp vào csdl
                var tintuc = new TinTuc()
                {
                    TinTucId = Guid.NewGuid().ToString(),
                    ThoiGian = DateTime.Now,
                    Url = GenerateUrl(ThemTinTuc.TieuDe),
                    TieuDe = ThemTinTuc.TieuDe,
                    NoiDung = ThemTinTuc.NoiDung,
                    NhanVienID = nhanvien.NhanVienID
                };

                // Lưu vào csdl
                _context.Add(tintuc);
                _context.SaveChanges();

                // Xử lý ảnh bìa bài viết (bắt buộc)
                if (AnhBia != null && AnhBia.Length > 0)
                {
                    var extension = Path.GetExtension(AnhBia.FileName);
                    var coverPath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "AnhBia");
                    string coverFile = await _fileManager.SaveArticleCoverAsync(AnhBia, coverPath, "TinTuc/AnhBia", tintuc.TinTucId);
                    if (!_fileManager.IsImageExtension(extension))
                    {
                        return View(ThemTinTuc);
                    }
                    tintuc.AnhBia = coverFile;
                    // Cập nhật trong csdl
                    _context.Update(tintuc);
                    _context.SaveChanges();
                }
                else
                {
                    _context.Remove(tintuc);
                    return View(ThemTinTuc);
                }

                // Xử lý các file trong bài viết (không bắt buộc)
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        if (_fileManager.IsImageExtension(extension))
                        {
                            var imgPath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "FileTinTuc", "Anh");
                            bool isSave = await _fileManager.SaveArticleFilesAsync(file, imgPath, (20 * 1024 * 1024), "TinTuc/Filetintuc/Anh", tintuc.TinTucId);
                            if (!isSave)
                            {
                                return View(ThemTinTuc);
                            }
                        }
                        else if (_fileManager.IsDocumentExtension(extension))
                        {
                            var docPath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "FileTinTuc", "Doc");
                            bool isSave = await _fileManager.SaveArticleFilesAsync(file, docPath, (20 * 1024 * 1024), "TinTuc/Filetintuc/Doc", tintuc.TinTucId);
                            if (!isSave)
                            {
                                return View(ThemTinTuc);
                            }
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            return View(ThemTinTuc);
        }

        [HttpPost]
        [Route("/staff/tin-tuc/chinh-sua")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChinhSua([Bind("TinTucId,TieuDe,NoiDung")] ChinhSuaTinTuc ChinhSuaTinTuc, List<IFormFile> files, IFormFile AnhBia = null)
        {
            if (ModelState.IsValid)
            {
                if (ChinhSuaTinTuc.TinTucId == null)
                {
                    return View(ChinhSuaTinTuc);
                }
                var _tintuc = _context.TinTucs.Find(ChinhSuaTinTuc.TinTucId);
                if (_tintuc == null)
                {
                    NotFound();
                }

                // Lấy thông tin nhân viên (đang đăng nhập hiện tại, người mà tương tác)
                var user = await _userManager.GetUserAsync(User);
                var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.AppUserId == user!.Id);
                if (nhanvien == null)
                {
                    // Liên hệ admin
                    return NotFound();
                }

                // Handle article cover (required)
                if (AnhBia != null && AnhBia.Length > 0)
                {
                    var extension = Path.GetExtension(AnhBia.FileName);
                    var coverPath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "AnhBia");
                    string coverFile = await _fileManager.SaveArticleCoverAsync(AnhBia, coverPath, "TinTuc/AnhBia", ChinhSuaTinTuc.TinTucId);
                    if (!_fileManager.IsImageExtension(extension))
                    {
                        Console.WriteLine("Wrong image type!");
                        return View(ChinhSuaTinTuc);
                    }

                    _tintuc!.AnhBia = coverFile;
                }

                // Handle all files in article (can null)
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        if (_fileManager.IsImageExtension(extension))
                        {
                            var imgPath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "FileTinTuc", "Anh");
                            bool isSave = await _fileManager.SaveArticleFilesAsync(file, imgPath, (20 * 1024 * 1024), "TinTuc/Filetintuc/Anh", ChinhSuaTinTuc.TinTucId);
                            if (!isSave)
                            {
                                return View(ChinhSuaTinTuc);
                            }
                        }
                        else if (_fileManager.IsDocumentExtension(extension))
                        {
                            var docPath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "FileTinTuc", "Doc");
                            bool isSave = await _fileManager.SaveArticleFilesAsync(file, docPath, (20 * 1024 * 1024), "TinTuc/Filetintuc/Doc", ChinhSuaTinTuc.TinTucId);
                            if (!isSave)
                            {
                                return View(ChinhSuaTinTuc);
                            }
                        }
                    }
                }

                // Điền hết thông tin còn thiếu
                _tintuc!.TieuDe = ChinhSuaTinTuc.TieuDe;
                _tintuc!.NoiDung = ChinhSuaTinTuc.NoiDung;
                _tintuc.Url = GenerateUrl(ChinhSuaTinTuc.TieuDe);
                //_context.Update(_tintuc);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(ChinhSua), new { id = ChinhSuaTinTuc.TinTucId });
        }

        [HttpPost]
        [Route("/staff/tin-tuc/xoa/{id?}")]
        [ValidateAntiForgeryToken]
        public IActionResult Xoa(string id)
        {
            var tintuc = _context.TinTucs.Include(a => a.FileTinTuc).FirstOrDefault(a => id == a.TinTucId);
            if (tintuc != null)
            {
                // Xóa các file đi kèm của bài viết
                foreach (var file in tintuc.FileTinTuc)
                {
                    _fileManager.DeleteFileDb(file.FileId);
                }

                // Xóa bài viết khỏi csdl
                _context.TinTucs.Remove(tintuc);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("/file/delete/{id?}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id)
        {
            var file = _context.FileTinTuc.Find(id);
            if (file != null)
            {
                _context.FileTinTuc.Remove(file);
                _context.SaveChanges();

                string filePath;
                if (_fileManager.IsImageExtension(Path.GetExtension(file.TenFile)))
                {
                    filePath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "FileTinTuc", "Anh", file.TenFile);
                }
                else if (_fileManager.IsDocumentExtension(Path.GetExtension(file.TenFile)))
                {
                    filePath = Path.Combine(_webHost.WebRootPath, "files", "TinTuc", "FileTinTuc", "Doc", file.TenFile);
                }
                else
                {
                    filePath = file.FilePath;
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return Ok();
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [HttpGet]
        public IActionResult TaiFile(string fileId)
        {
            var file = _context.FileTinTuc.FirstOrDefault(f => f.FileId == fileId);
            if (file == null)
            {
                return NotFound();
            }

            var filePath = file.FilePath;
            var fileName = file.TenFile;

            var fullPath = Path.Combine(_webHost.WebRootPath, filePath.Replace('/', '\\'));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var contentType = _fileManager.GetContentType(fileName);
            return PhysicalFile(fullPath, contentType, fileName);
        }
    }
}