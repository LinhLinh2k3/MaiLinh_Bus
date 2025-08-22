using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using System.Linq;
using System.Threading.Tasks;
using NhaXeMaiLinh.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Identity;
using NhaXeMaiLinh.Services;

namespace NhaXeMaiLinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public NhanVienController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
     
        [Route("/admin/nhanvien/danh-sach-nhan-vien")]
        public async Task<IActionResult> Index()
        {
            var nhanViens = await _context.NhanViens.Where(nv => nv.AppUser.isEnabled == true).ToListAsync();
            return View(nhanViens);
        }
        //        CREATE PROCEDURE GetNhanVienDetails
        //    @NhanVienID NVARCHAR(50)
        //AS
        //BEGIN
        //    SELECT*
        //    FROM NhanViens
        //    WHERE NhanVienID = @NhanVienID
        //END
        private bool NhanVienExists(string id)
        {
            return _context.NhanViens.Any(e => e.NhanVienID == id);
        }
        [Route("/admin/nhanvien/chi-tiet-nhan-vien/{id}")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/nhanvien/chi-tiet-nhan-vien/{id}")]
        public async Task<IActionResult> Edit(string id, [Bind("NhanVienID,HoTen,NgaySinh,GioiTinh,DiaChi,SDT,Email,CCCD,NgayVaoLam,ChucVu,AppUserId")] NhanVien nhanVien)
        {
            if (id != nhanVien.NhanVienID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVien);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.NhanVienID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(nhanVien);
        }
        [HttpGet]
        [Route("/admin/nhanvien/tao-nhan-vien")]
        public IActionResult Create()
        {
            return View();
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/nhanvien/tao-nhan-vien")]
        public async Task<IActionResult> Create(CreateNhanVienViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new AppUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        Name = model.HoTen,
                        Address = model.DiaChi,
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = false,
                        AccessFailedCount = 0,
                        isEnabled=true
                    };

                    //var defaultPassword = model.Email.Split('@')[0];
                    var defaultPassword = model.Email + "@123"; // Thêm ký tự để tăng độ dài và phức tạp
                    var result = await _userManager.CreateAsync(user, defaultPassword);

                    if (result.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync("NhanVien"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("NhanVien"));
                        }
                        await _userManager.AddToRoleAsync(user, "NhanVien");

                        var nhanVien = new NhanVien
                        {
                            NhanVienID = Guid.NewGuid().ToString(),
                            HoTen = model.HoTen,
                            Email = model.Email,
                            DiaChi = model.DiaChi,
                            AppUserId = user.Id
                        };
                        _context.NhanViens.Add(nhanVien);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Tạo tài khoản nhân viên thành công!";
                        return View(model); 
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình tạo tài khoản.");

                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/nhanvien/xoa-nhan-vien/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID nhân viên không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(nhanVien.AppUserId);
            if (user != null)
            {
                user.isEnabled = false;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Xóa nhân viên thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản người dùng.";
            }

           
           return LocalRedirect("/admin/nhanvien/danh-sach-nhan-vien");
        }
        [Route("/admin/nhanvien/danh-sach-da-xoa")]
        public async Task<IActionResult> nhanvienxoa()
        {
            var nhanViens = await _context.NhanViens.Where(nv => nv.AppUser.isEnabled == false).ToListAsync();
            return View(nhanViens); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/nhanvien/khoi-phuc-nhan-vien/{id}")]
        public async Task<IActionResult> Restore(string id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy nhân viên.";
                return RedirectToAction("nhanvienxoa");
            }

            var user = await _userManager.FindByIdAsync(nhanVien.AppUserId);
            if (user != null)
            {
                user.isEnabled = true;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Khôi phục nhân viên thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản người dùng.";
            }

            return RedirectToAction("nhanvienxoa");
        }


    }
}
