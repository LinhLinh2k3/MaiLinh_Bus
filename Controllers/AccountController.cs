using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using NhaXeMaiLinh.ViewModels;
using System.Security.Claims;
using NhaXeMaiLinh.Services.Email;

namespace NhaXeMaiLinh.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IEmailSender emailSender;
       

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ApplicationDbContext dbContext, IEmailSender emailSender)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.dbContext = dbContext;
            this.emailSender = emailSender;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by username
                var user = await userManager.FindByNameAsync(model.Username);
                if (user != null && user.isEnabled == true)
                {
                    // Login
                    var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (user.isEnabled == false)
                        {
                            // Tài khoản đã bị xóa
                            return View();
                        }

                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return LocalRedirect("/admin/login");
                        }
                        else if (await userManager.IsInRoleAsync(user, "NhanVien"))
                        {
                            return LocalRedirect("/staff/dashboard");
                        }
                        else
                        {
                            return RedirectToAction("TrangChu", "Home");
                        }
                    }
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }
            return View(model);
        }

        //public async Task<string> GetUserFullName()
        //{
        //    var user = await userManager.GetUserAsync(User);
        //    if (user != null)
        //    {
        //        var khachHang = await dbContext.KhachHangs.FirstOrDefaultAsync(k => k.AppUserId == user.Id);
        //        if (khachHang != null)
        //        {
        //            return khachHang.HoTen;
        //        }
        //    }
        //    return string.Empty;
        //}
        [Route("/dang-ky")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/dang-ky")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng AppUser
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Address = model.Address,
                    isEnabled = true
                };

                // Tạo người dùng mới
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Gán role KhachHang cho người dùng
                    await userManager.AddToRoleAsync(user, "KhachHang");

                    // Tạo đối tượng KhachHang và lưu vào cơ sở dữ liệu
                    var khachHang = new KhachHang
                    {
                        HoTen = model.Name,
                        Email = model.Email,
                        DiaChi = model.Address,
                        HangThanhVien = "Phổ thông",
                        AppUserId = user.Id // Liên kết với AppUser
                    };

                    dbContext.KhachHangs.Add(khachHang);
                    await dbContext.SaveChangesAsync();

                    // Đăng nhập tự động (nếu cần)
                    await signInManager.SignInAsync(user, isPersistent: false);

                    // Chuyển hướng đến trang chủ sau khi đăng ký thành công
                    return RedirectToAction("TrangChu", "Home");
                }

                // Nếu có lỗi, hiển thị lỗi
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }


            return View(model);
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("TrangChu", "Home");
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            // Cấu hình các thuộc tính xác thực bên ngoài
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            // Thực hiện xác thực với nhà cung cấp bên ngoài
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login");
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Không thể lấy thông tin đăng nhập từ nhà cung cấp.");
                return View("Login");
            }

            // Xử lý đăng nhập
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            // Tạo người dùng mới
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name); // Lấy tên của người dùng Google

            if (email != null)
            {
                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    Name = name, // Lưu tên Google vào cột 'Name'
                    isEnabled = true
                };

                var createResult = await userManager.CreateAsync(user);

                if (createResult.Succeeded)
                {
                    // Gán role KhachHang cho người dùng
                    await userManager.AddToRoleAsync(user, "KhachHang");

                    var loginResult = await userManager.AddLoginAsync(user, info);
                    if (loginResult.Succeeded)
                    {
                        // Tạo đối tượng KhachHang và lưu vào cơ sở dữ liệu
                        KhachHang khachHang = new()
                        {
                            HoTen = name,
                            Email = email,
                            //DiaChi = "", // You can set a default value or leave it empty
                            AppUserId = user.Id
                        };

                        dbContext.KhachHangs.Add(khachHang);
                        await dbContext.SaveChangesAsync();

                        await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View("Login");
        }

        //Xem thông tin khách hàng
        [HttpGet]
        public async Task<IActionResult> ViewCustomerInfo()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var khachHang = await dbContext.KhachHangs
                .FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        //Update Customer Information
        [HttpGet]
        public async Task<IActionResult> UpdateCustomerInfo()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var khachHang = await dbContext.KhachHangs
                .FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            if (khachHang == null)
            {
                return NotFound();
            }

            var model = new UpdateProfileVM
            {
                HoTen = khachHang.HoTen,
                SDT = khachHang.SDT,
                Email = khachHang.Email,
                HangThanhVien = khachHang.HangThanhVien
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomerInfo(UpdateProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var khachHang = await dbContext.KhachHangs
                .FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            if (khachHang == null)
            {
                return NotFound();
            }

            khachHang.HoTen = model.HoTen;
            khachHang.SDT = model.SDT;
            khachHang.Email = model.Email;
            khachHang.HangThanhVien = model.HangThanhVien;

            dbContext.KhachHangs.Update(khachHang);
            await dbContext.SaveChangesAsync();
			TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";
            //return RedirectToAction("ViewCustomerInfo");
            return View(model);
        }
        private async Task<bool> IsExternalLoginAsync(AppUser user)
        {
            var logins = await userManager.GetLoginsAsync(user);
            return logins.Any();
        }



        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Kiểm tra xem người dùng có đăng nhập bằng nhà cung cấp bên ngoài không
            if (await IsExternalLoginAsync(user))
            {
                TempData["IsExternalLogin"] = true;
                TempData["ErrorMessage"] = "Bạn không thể thay đổi mật khẩu vì đăng nhập bằng google";
                return View();
            }
            TempData["IsExternalLogin"] = false;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Kiểm tra xem người dùng có đăng nhập bằng nhà cung cấp bên ngoài không
            if (await IsExternalLoginAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Bạn không thể thay đổi mật khẩu vì đăng nhập bằng google");
                return View(model);
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Mật khẩu đã thay đổi thành công!";
                // return RedirectToAction("ChangePasswordConfirmation");
                return View();

            }
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [Route("/ve-cua-ban")]
        public async Task<IActionResult> ViewTickets()
        {
            //   var user = await userManager.GetUserAsync(User);
            //   //if (user == null)
            //   //{
            //   //    return RedirectToAction("Login", "Account");
            //   //}

            //   var khachHang = await dbContext.KhachHangs
            //       .FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            //   if (khachHang == null)
            //   {
            //       return NotFound();
            //   }

            //   var veXes = await dbContext.VeXes
            //.Where(v => v.KhachHangID == khachHang.KhachHangID)
            //.Include(v => v.LichTrinh)
            //    .ThenInclude(lt => lt.TuyenDuong) // Ensure TuyenDuong is included
            //.Include(v => v.KhuyenMai)
            //.ToListAsync();

            //   return View(veXes);


            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var khachHang = await dbContext.KhachHangs
                .FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            if (khachHang == null)
            {
                return NotFound();
            }

            var veXes = await dbContext.VeXes
                .Where(v => v.KhachHangID == khachHang.KhachHangID && v.isDelete == false)
                .Include(v => v.LichTrinh)
                    .ThenInclude(lt => lt.TuyenDuong) // Ensure TuyenDuong is included
                .Include(v => v.KhuyenMai)
                .Include(v => v.ChiTietVeDats) // Include ChiTietVeDats
                .ThenInclude(ct => ct.Ghe)
                .ToListAsync();

            return View(veXes);

        }
        [Route("/lich-su-giao-dich")]
        public async Task<IActionResult> TransactionHistory()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var khachHang = await dbContext.KhachHangs
                .FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            if (khachHang == null)
            {
                return NotFound();
            }

            var transactions = await dbContext.LichSuGiaoDichs
                .Where(t => t.VeXe.KhachHangID == khachHang.KhachHangID)
                .Include(t => t.VeXe)
                .Include(t => t.NhanVien)
                .ToListAsync();

            return View(transactions);
        }

        // GET: /Account/ForgotPassword
        [Route("/quen-mat-khau")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/quen-mat-khau")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && user.isEnabled == true)
                {
                    // Generate password reset token
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = user.Email }, Request.Scheme);

                    // Send email with reset link
                 //  await emailSender.SendEmail(model.Email, "Reset Password", $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>");
                    await emailSender.SendEmail(model.Email, "Đặt lại Mật khẩu", $"Nhấn vào liên kết dưới đây để đặt lại mật khẩu của bạn: <a href='{resetLink}'>Đặt lại mật khẩu</a>");

                    TempData["Message"] = "Email đặt lại mật khẩu đã được gửi.";
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // If user doesn't exist or is disabled
                ModelState.AddModelError(string.Empty, "Email không hợp lệ.");
            }

            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        TempData["Message"] = "Mật khẩu của bạn đã được đặt lại";
                        return RedirectToAction("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email không hợp lệ.");
                }
            }
            return View(model);
        }

        [Route("/ve-cua-ban/chi-tiet")]
        public async Task<IActionResult> ChiTietVe(string veID)
        {
            var veXe = await dbContext.VeXes
                .Include(v => v.LichTrinh)
                    .ThenInclude(lt => lt.TuyenDuong)
                .Include(v => v.LichTrinh)
                    .ThenInclude(lt => lt.Xe)
                .Include(v => v.KhuyenMai)
                .Include(v => v.ChiTietVeDats)
                    .ThenInclude(ct => ct.Ghe) // Đảm bảo bao gồm thông tin về ghế
                .FirstOrDefaultAsync(v => v.VeID == veID);

            if (veXe == null)
            {
                return NotFound();
            }

            return View(veXe); // Trả về View chi tiết vé
        }
    }
}
