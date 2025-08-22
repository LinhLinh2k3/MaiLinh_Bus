using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NhaXeMaiLinh.Models.Data;

namespace NhaXeMaiLinh.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TinTuc> TinTucs { get; set; }
        public DbSet<FileTinTuc> FileTinTuc { get; set; }
        public DbSet<TaiXe> TaiXes { get; set; }
        public DbSet<Xe> Xes { get; set; }
        public DbSet<TuyenDuong> TuyenDuongs { get; set; }
        public DbSet<LichTrinh> LichTrinhs { get; set; }
        public DbSet<VeXe> VeXes { get; set; }
        public DbSet<ChiTietVeDat> ChiTietVeDats { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<NhanXetKhachHang> NhanXetKhachHangs { get; set; }
        public DbSet<Ghe> Ghes { get; set; }
        public DbSet<LichSuGiaoDich> LichSuGiaoDichs { get; set; }
        public DbSet<LichSuDoiGhe> LichSuDoiGhes { get; set; }
        public DbSet<LoaiXe> LoaiXes { get; set; } 
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<KhuyenMai_KH> KhuyenMai_KHs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LichSuDoiGhe>(e =>
            {
                e.HasOne(x => x.LichSuGiaoDich)
                 .WithMany()
                 .HasForeignKey(x => x.GiaoDichID)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<LichSuDoiGhe>().Property(e => e.DoiGheID).ValueGeneratedOnAdd();

            builder.Entity<KhachHang>()
                .HasOne(k => k.AppUser)
                .WithOne(a => a.KhachHang)
                .HasForeignKey<KhachHang>(k => k.AppUserId);

            builder.Entity<Ghe>(entity =>
            {
                entity.HasKey(g => new { g.GheID, g.XeID });
            });

            builder.Entity<KhuyenMai_KH>(entity =>
            {
                entity.HasKey(g => new { g.KhuyenMaiID, g.KhachHangID });
            });
        }
        public async Task<NhanVien> GetNhanVienDetailsAsync(string nhanVienID)
        {
            var nhanVienIDParam = new SqlParameter("@NhanVienID", nhanVienID);
            var result = await NhanViens
                .FromSqlRaw("EXEC GetNhanVienDetails @NhanVienID", nhanVienIDParam)
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}