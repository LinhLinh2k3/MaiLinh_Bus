using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Areas.Staff.Models;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using System.Security.Claims;

namespace NhaXeMaiLinh.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "NhanVien")]
    public class VeXeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeXeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/staff/ve-xe/quan-ly")]
        public async Task<IActionResult> Index()
        {
            var vexes = _context.VeXes.Include(v => v.LichTrinh.TuyenDuong)
                .Include(v => v.KhachHang)
                .Include(v => v.KhuyenMai)
                .Include(v => v.LichTrinh)
                .Where(v => v.isDelete != true); // Không hiện mấy thằng bị xóa

            return View(await vexes.ToListAsync());
        }

        [Route("/staff/ve-xe/chi-tiet/{id?}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veXe = await _context.VeXes
                .Include(v => v.KhachHang)
                .Include(v => v.KhuyenMai)
                .Include(v => v.LichTrinh.TuyenDuong)
                .Include(v => v.ChiTietVeDats)
                .FirstOrDefaultAsync(m => m.VeID == id);
            if (veXe == null)
            {
                return NotFound();
            }

            return View(veXe);
        }

        [Route("/staff/ve-xe/them")]
        public IActionResult Create()
        {
            var dskm = _context.KhuyenMais.Select(km => new SelectListItem
            {
                Value = km.KhuyenMaiID,
                Text = km.TenKhuyenMai
            });

            ViewData["KhuyenMaiID"] = new SelectList(dskm, "Value", "Text");
            return View();
        }

        [Route("/staff/ve-xe/them")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VeID,KhachHangID,LichTrinhID,KhuyenMaiID,TongGiaVe")] VeModel veXe, List<int> dsGhe)
        {
            if (ModelState.IsValid)
            {
                var createdTime = DateTime.Now;
                var nhanvien = _context.NhanViens.First(n => n.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
                string nhanvienId = nhanvien.NhanVienID;

                // Tạo thông tin vé xe
                var vexe = new VeXe()
                {
                    VeID = veXe.VeID,
                    KhachHangID = veXe.KhachHangID,
                    LichTrinhID = veXe.LichTrinhID,
                    KhuyenMaiID = veXe.KhuyenMaiID,
                    TongGiaVe = veXe.TongGiaVe
                };
                _context.Add(vexe);

                // Cập nhật ghế khi đã chốt thành công
                foreach (var gheId in dsGhe)
                {
                    var ghe = _context.Ghes.FirstOrDefault(g => g.GheID == gheId);
                    if (ghe != null)
                    {
                        ghe.TrangThai = "mua";
                        _context.Update(ghe);

                        var chitietvedat = new ChiTietVeDat()
                        {
                            ChiTietVeID = Guid.NewGuid().ToString(),
                            VeID = veXe.VeID,
                            SoGhe = ghe.GheID,
                            XeID = ghe.XeID,
                            GiaGhe = (decimal)_context.LichTrinhs.First(l => l.LichTrinhId == veXe.LichTrinhID).GiaVe,
                            TinhTrangGhe = "đã mua",
                            NgayDat = createdTime
                        };
                        _context.Add(chitietvedat);
                    }
                }

                // Lưu lại lịch sử giao dịch
                var lsgd = new LichSuGiaoDich()
                {
                    GiaoDichID = Guid.NewGuid().ToString(),
                    VeID = veXe.VeID,
                    LoaiGiaoDich = null,
                    ChiTiet = "Giao dịch bởi nhân viên " + nhanvien.HoTen,
                    NgayGiaoDich = createdTime,
                    NhanVienID = nhanvienId,
                    TrangThaiGiaoDich = "chờ thanh toán"
                };
                _context.Add(lsgd);

                // Xuất hóa đơn
                var hoadon = new HoaDon()
                {
                    VeID = veXe.VeID,
                    NhanVienID = nhanvienId,
                    NgayLap = createdTime,
                    TongTien = veXe.TongGiaVe,
                    PhuongThucThanhToan = "chưa có",
                    TrangThaiThanhToan = "chờ thanh toán"
                };
                _context.Add(hoadon);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var dskm = _context.KhuyenMais.Select(km => new SelectListItem
            {
                Value = km.KhuyenMaiID,
                Text = km.TenKhuyenMai
            });

            ViewData["KhuyenMaiID"] = new SelectList(dskm, "Value", "Text");
            return View(veXe);
        }

        [Route("/staff/ve-xe/chinh-sua/{id?}")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veXe = await _context.VeXes.FindAsync(id);
            if (veXe == null)
            {
                return NotFound();
            }

            var dskh = _context.KhachHangs.Where(k => k.KhachHangID == veXe.KhachHangID).Select(k => new SelectListItem
            {
                Value = k.KhachHangID,
                Text = $"{k.HoTen} - {k.CCCD}"
            });

            var dsLichTrinh = _context.LichTrinhs.Where(l => l.LichTrinhId == veXe.LichTrinhID).Include(l => l.TuyenDuong).Select(l => new SelectListItem
            {
                Value = l.LichTrinhId,
                Text = $"{l.TuyenDuong.DiemDi} - {l.TuyenDuong.DiemDen} | {l.GioKhoiHanh} - {l.GioDen}"
            });

            var dskm = _context.KhuyenMais.Select(km => new SelectListItem
            {
                Value = km.KhuyenMaiID,
                Text = km.TenKhuyenMai
            });

            ViewData["KhachHangID"] = new SelectList(dskh, "Value", "Text");
            ViewData["LichTrinhID"] = new SelectList(dsLichTrinh, "Value", "Text");
            ViewData["KhuyenMaiID"] = new SelectList(dskm, "Value", "Text", veXe.KhuyenMaiID);
            return View(veXe);
        }

        [Route("/staff/ve-xe/chinh-sua/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("VeID,KhachHangID,LichTrinhID,KhuyenMaiID,TongGiaVe")] VeModel veXe, int GheCu, int GheMoi, string LyDo)
        {
            if (id != veXe.VeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var createdTime = DateTime.Now;
                    var nhanvien = _context.NhanViens.First(n => n.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
                    string nhanvienId = nhanvien.NhanVienID;

                    // Cập nhật cái vé trước
                    var vexe = _context.VeXes.FirstOrDefault(v => v.VeID == veXe.VeID);
                    if (vexe != null)
                    {
                        vexe.KhuyenMaiID = veXe.KhuyenMaiID;
                        vexe.TongGiaVe = veXe.TongGiaVe;
                        _context.Update(vexe);
                    }

                    // Cập nhật thông tin ghế phát
                    var ghemoi = _context.Ghes.FirstOrDefault(g => g.GheID == GheMoi);
                    if (ghemoi != null)
                    {
                        ghemoi.TrangThai = "mua";
                        _context.Update(ghemoi);

                        // Thêm chi tiết vé đặt
                        var chitietvedat = new ChiTietVeDat()
                        {
                            ChiTietVeID = Guid.NewGuid().ToString(),
                            VeID = veXe.VeID,
                            SoGhe = ghemoi.GheID,
                            XeID = ghemoi.XeID,
                            GiaGhe = (decimal)_context.LichTrinhs.First(l => l.LichTrinhId == veXe.LichTrinhID).GiaVe,
                            TinhTrangGhe = "chờ thanh toán",
                            NgayDat = createdTime
                        };
                        _context.Add(chitietvedat);
                    }

                    // Lịch sử giao dịch cái mới
                    var lsgd = new LichSuGiaoDich()
                    {
                        GiaoDichID = Guid.NewGuid().ToString(),
                        VeID = veXe.VeID,
                        LoaiGiaoDich = null,
                        ChiTiet = "Giao dịch bởi nhân viên " + nhanvien.HoTen,
                        NgayGiaoDich = createdTime,
                        NhanVienID = nhanvienId,
                        TrangThaiGiaoDich = "chờ thanh toán"
                    };
                    _context.Add(lsgd);

                    // Loại mấy ghế hủy đi
                    var ghe = _context.Ghes.FirstOrDefault(g => g.GheID == GheCu);
                    if (ghe != null)
                    {
                        ghe.TrangThai = "trống";
                        _context.Update(ghe);

                        // Gắn cái chi tiết vé đặt của cái cũ
                        var chitietvedat = _context.ChiTietVeDats.FirstOrDefault(c => c.SoGhe == GheCu);
                        if (chitietvedat != null)
                        {
                            // Thêm lịch sử đổi ghế nè
                            var lsdg = new LichSuDoiGhe()
                            {
                                ChiTietVeID = chitietvedat.ChiTietVeID,
                                GheCu = GheCu,
                                GheMoi = GheMoi,
                                NgayDoi = DateTime.Now,
                                LyDoDoi = LyDo,
                                NhanVienID = nhanvienId,
                                GiaoDichID = lsgd.GiaoDichID
                            };
                            _context.Add(lsdg);
                        }
                    }

                    // Cập nhật hóa đơn nha
                    var hoadon = _context.HoaDons.First(h => h.VeID == vexe.VeID);
                    hoadon.TongTien = veXe.TongGiaVe;
                    _context.Update(hoadon);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeXeExists(veXe.VeID))
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

            var dskh = _context.KhachHangs.Select(k => new SelectListItem
            {
                Value = k.KhachHangID,
                Text = $"{k.HoTen} - {k.CCCD}"
            });

            var dsLichTrinh = _context.LichTrinhs.Include(l => l.TuyenDuong).Select(l => new SelectListItem
            {
                Value = l.LichTrinhId,
                Text = $"{l.TuyenDuong.DiemDi} - {l.TuyenDuong.DiemDen} | {l.GioKhoiHanh} - {l.GioDen}"
            });

            var dskm = _context.KhuyenMais.Select(km => new SelectListItem
            {
                Value = km.KhuyenMaiID,
                Text = km.TenKhuyenMai
            });

            ViewData["KhachHangID"] = new SelectList(dskh, "Value", "Text", veXe.KhachHangID);
            ViewData["LichTrinhID"] = new SelectList(dsLichTrinh, "Value", "Text", veXe.LichTrinhID);
            ViewData["KhuyenMaiID"] = new SelectList(dskm, "Value", "Text", veXe.KhuyenMaiID);
            return View(veXe);
        }

        [Route("/staff/ve-xe/huy/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var veXe = await _context.VeXes.FindAsync(id);
            if (veXe != null)
            {
                var createdTime = DateTime.Now;
                var nhanvien = _context.NhanViens.First(n => n.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Chuyển vé xe và chi tiết vé đặt liên quan tới vé xe này về trạng thái không hoạt động! (Chính sách)
                var chitietvedat = _context.ChiTietVeDats.Where(c => c.VeID == id).ToList();
                foreach (var ctvd in chitietvedat)
                {
                    // Trả mấy cái ghế về trạng thái trống
                    var ghe = _context.Ghes.First(g => g.GheID == ctvd.SoGhe);
                    if (ghe != null)
                    {
                        ghe.TrangThai = "trống";
                        _context.Update(ghe);
                    }

                    ctvd.TinhTrangGhe = "đã hủy";
                    _context.Update(ctvd);
                }

                var lsgd = new LichSuGiaoDich()
                {
                    GiaoDichID = Guid.NewGuid().ToString(),
                    VeID = veXe.VeID,
                    LoaiGiaoDich = "cancel",
                    ChiTiet = "Hủy vé với mã: " + veXe.VeID,
                    NgayGiaoDich = createdTime,
                    NhanVienID = nhanvien.NhanVienID,
                    TrangThaiGiaoDich = "chưa thanh toán"
                };
                _context.Add(lsgd);

                // PP HoaDon
                var hoadon = _context.HoaDons.First(h => h.VeID == id);
                _context.Remove(hoadon);

                // Xóa vé oy (Admin mới thấy)
                veXe.isDelete = true;
                _context.Update(veXe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("/staff/ve-xe/duyet")]
        [HttpGet]
        public async Task<IActionResult> CheckAsync()
        {
            var vexes = _context.VeXes.Include(v => v.LichTrinh.TuyenDuong)
                .Include(v => v.KhachHang)
                .Include(v => v.KhuyenMai)
                .Include(v => v.LichTrinh)
                .Where(v => v.isDelete == null); // Hien may thang cho duyet

            return View(await vexes.ToListAsync());
        }

        [Route("/staff/ve-xe/chap-nhan/{id}")]
        public IActionResult Accept(string id)
        {
            var ve = _context.VeXes.Find(id);
            if (ve == null) return NotFound();

            ve.isDelete = true;
            _context.Update(ve);

            var lsgd = new LichSuGiaoDich()
            {
                GiaoDichID = Guid.NewGuid().ToString(),
                VeID = ve.VeID,
                LoaiGiaoDich = "Cancel",
                ChiTiet = "Hủy vé theo mã: " + ve.VeID,
                NgayGiaoDich = DateTime.Now,
                NhanVienID = _context.NhanViens.First(n => n.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).NhanVienID,
                TrangThaiGiaoDich = null
            };
            _context.Add(lsgd);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Route("/staff/ve-xe/tu-choi/{id}")]
        public IActionResult Deny(string id)
        {
            var ve = _context.VeXes.Find(id);
            if (ve == null) return NotFound();

            ve.isDelete = false;
            _context.Update(ve);

            var lsgd = new LichSuGiaoDich()
            {
                GiaoDichID = Guid.NewGuid().ToString(),
                VeID = ve.VeID,
                LoaiGiaoDich = "Cancel",
                ChiTiet = "Hủy vé theo mã: " + ve.VeID,
                NgayGiaoDich = DateTime.Now,
                NhanVienID = _context.NhanViens.First(n => n.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier)).NhanVienID,
                TrangThaiGiaoDich = null
            };
            _context.Add(lsgd);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool VeXeExists(string id)
        {
            return _context.VeXes.Any(e => e.VeID == id);
        }
    }
}