using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NhaXeMaiLinh.Data;
using NhaXeMaiLinh.Models.Data;
using NhaXeMaiLinh.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using NhaXeMaiLinh.Services.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

//// Multiple language
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new("en"),
        new("vi")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});




// Đăng ký HttpClient
builder.Services.AddHttpClient();

builder.Services.AddScoped<FileManager>();

builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<Chatbot>();
builder.Services.AddTransient<Chatbot>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Create roles and assign them to users
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    string[] roleNames = { "KhachHang", "NhanVien", "Admin" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Tạo tài khoản cho các role nếu chưa có
    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Name = "Admin User",
            isEnabled = true
        };
        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }



    var nhanVienEmail = "nhanvien@gmail.com";
    var nhanVienUser = await userManager.FindByEmailAsync(nhanVienEmail);
    if (nhanVienUser == null)
    {
        nhanVienUser = new AppUser
        {
            UserName = nhanVienEmail,
            Email = nhanVienEmail,
            Name = "Nhan Vien User",
            isEnabled = true

        };
        var result = await userManager.CreateAsync(nhanVienUser, "NhanVien@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(nhanVienUser, "NhanVien");
            // Tạo đối tượng NhanVien và lưu vào cơ sở dữ liệu
            var nhanVien = new NhanVien
            {
                NhanVienID = Guid.NewGuid().ToString(), // Generate a unique ID
                HoTen = "Nhan Vien User",
                Email = nhanVienEmail,
                AppUserId = nhanVienUser.Id // Set the foreign key
            };
            dbContext.NhanViens.Add(nhanVien);
            await dbContext.SaveChangesAsync();
        }
    }

    var khachHangEmail = "khachhang@gmail.com";
    var khachHangUser = await userManager.FindByEmailAsync(khachHangEmail);
    if (khachHangUser == null)
    {
        khachHangUser = new AppUser
        {
            UserName = khachHangEmail,
            Email = khachHangEmail,
            Name = "Khach Hang User",
            isEnabled = true
        };
        var result = await userManager.CreateAsync(khachHangUser, "KhachHang@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(khachHangUser, "KhachHang");
            // Tạo đối tượng KhachHang và lưu vào cơ sở dữ liệu
            var khachHang = new KhachHang
            {
                HoTen = "Khach Hang",
                Email = khachHangEmail,
                AppUserId = khachHangUser.Id
            };
            dbContext.KhachHangs.Add(khachHang);
            await dbContext.SaveChangesAsync();
        }
    }
    // Insert data into LoaiXe table
    if (!dbContext.LoaiXes.Any())
    {
        dbContext.LoaiXes.AddRange(
            new LoaiXe { LoaiXeID = "LX001", TenLoaiXe = "Xe Giường Nằm", HangXe = "Hyundai", SLGhe = 40 },
            new LoaiXe { LoaiXeID = "LX002", TenLoaiXe = "Xe Ghế Ngồi", HangXe = "Toyota", SLGhe = 30 },
            new LoaiXe { LoaiXeID = "LX003", TenLoaiXe = "Xe VIP", HangXe = "Mercedes", SLGhe = 20 },
            new LoaiXe { LoaiXeID = "LX004", TenLoaiXe = "Xe Thường", HangXe = "Kia", SLGhe = 30 },
            new LoaiXe { LoaiXeID = "LX005", TenLoaiXe = "Xe Cao Cấp", HangXe = "Ford", SLGhe = 35 }
        );
        await dbContext.SaveChangesAsync();
    }
}
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");


    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapDefaultControllerRoute();
});
app.Run();
