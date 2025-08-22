using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class createdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangs_TaiKhoans_TKId",
                table: "KhachHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_NhanViens_TaiKhoans_TKId",
                table: "NhanViens");

            migrationBuilder.DropIndex(
                name: "IX_NhanViens_TKId",
                table: "NhanViens");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangs_TKId",
                table: "KhachHangs");

            migrationBuilder.DropColumn(
                name: "TKId",
                table: "NhanViens");

            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "KhachHangs");

            migrationBuilder.DropColumn(
                name: "NgaySinh",
                table: "KhachHangs");

            migrationBuilder.DropColumn(
                name: "TKId",
                table: "KhachHangs");

            migrationBuilder.RenameColumn(
                name: "NhanVienId",
                table: "NhanViens",
                newName: "NhanVienID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgayVaoLam",
                table: "NhanViens",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySinh",
                table: "NhanViens",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "TaiXes",
                columns: table => new
                {
                    TaiXeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BangLaiXe = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiXes", x => x.TaiXeID);
                });

            migrationBuilder.CreateTable(
                name: "TuyenDuongs",
                columns: table => new
                {
                    TuyenDuongID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenTuyenDuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemDi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemDen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuangDuong = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TuyenDuongs", x => x.TuyenDuongID);
                });

            migrationBuilder.CreateTable(
                name: "Xes",
                columns: table => new
                {
                    XeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BienSo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiXe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoGhe = table.Column<int>(type: "int", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaoDuong = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Xes", x => x.XeID);
                });

            migrationBuilder.CreateTable(
                name: "BaoDuongs",
                columns: table => new
                {
                    BaoDuongID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    XeID = table.Column<int>(type: "int", nullable: false),
                    NgayBaoDuong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoaiBaoDuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaSuCo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaoDuongs", x => x.BaoDuongID);
                    table.ForeignKey(
                        name: "FK_BaoDuongs_Xes_XeID",
                        column: x => x.XeID,
                        principalTable: "Xes",
                        principalColumn: "XeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichTrinhs",
                columns: table => new
                {
                    LichTrinhId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    XeId = table.Column<int>(type: "int", nullable: false),
                    TuyenXe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TuyenDuongId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GioKhoiHanh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioDen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichTrinhs", x => x.LichTrinhId);
                    table.ForeignKey(
                        name: "FK_LichTrinhs_TuyenDuongs_TuyenDuongId",
                        column: x => x.TuyenDuongId,
                        principalTable: "TuyenDuongs",
                        principalColumn: "TuyenDuongID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichTrinhs_Xes_XeId",
                        column: x => x.XeId,
                        principalTable: "Xes",
                        principalColumn: "XeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhanXetKhachHangs",
                columns: table => new
                {
                    NhanXetID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LichTrinhID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DanhGia = table.Column<int>(type: "int", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanXetKhachHangs", x => x.NhanXetID);
                    table.ForeignKey(
                        name: "FK_NhanXetKhachHangs_KhachHangs_KhachHangID",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NhanXetKhachHangs_LichTrinhs_LichTrinhID",
                        column: x => x.LichTrinhID,
                        principalTable: "LichTrinhs",
                        principalColumn: "LichTrinhId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VeXes",
                columns: table => new
                {
                    VeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KhachHangID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LichTrinhID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TongGiaVe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiVe = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeXes", x => x.VeID);
                    table.ForeignKey(
                        name: "FK_VeXes_KhachHangs_KhachHangID",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VeXes_LichTrinhs_LichTrinhID",
                        column: x => x.LichTrinhID,
                        principalTable: "LichTrinhs",
                        principalColumn: "LichTrinhId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietVeDats",
                columns: table => new
                {
                    ChiTietVeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoGhe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaGhe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TinhTrangGhe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VeXeVeID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietVeDats", x => x.ChiTietVeID);
                    table.ForeignKey(
                        name: "FK_ChiTietVeDats_VeXes_VeXeVeID",
                        column: x => x.VeXeVeID,
                        principalTable: "VeXes",
                        principalColumn: "VeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    HoaDonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NhanVienID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VeXeVeID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.HoaDonID);
                    table.ForeignKey(
                        name: "FK_HoaDons_NhanViens_NhanVienID",
                        column: x => x.NhanVienID,
                        principalTable: "NhanViens",
                        principalColumn: "NhanVienID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDons_VeXes_VeXeVeID",
                        column: x => x.VeXeVeID,
                        principalTable: "VeXes",
                        principalColumn: "VeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuGiaoDichs",
                columns: table => new
                {
                    GiaoDichID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiGiaoDich = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiTiet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGiaoDich = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NhanVienID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrangThaiGiaoDich = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VeXeVeID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuGiaoDichs", x => x.GiaoDichID);
                    table.ForeignKey(
                        name: "FK_LichSuGiaoDichs_NhanViens_NhanVienID",
                        column: x => x.NhanVienID,
                        principalTable: "NhanViens",
                        principalColumn: "NhanVienID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LichSuGiaoDichs_VeXes_VeXeVeID",
                        column: x => x.VeXeVeID,
                        principalTable: "VeXes",
                        principalColumn: "VeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuDoiGhes",
                columns: table => new
                {
                    DoiGheID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChiTietVeID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GheCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GheMoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayDoi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDoDoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChenhLechGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NhanVienID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GiaoDichID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChiTietVeDatChiTietVeID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuDoiGhes", x => x.DoiGheID);
                    table.ForeignKey(
                        name: "FK_LichSuDoiGhes_ChiTietVeDats_ChiTietVeDatChiTietVeID",
                        column: x => x.ChiTietVeDatChiTietVeID,
                        principalTable: "ChiTietVeDats",
                        principalColumn: "ChiTietVeID");
                    table.ForeignKey(
                        name: "FK_LichSuDoiGhes_LichSuGiaoDichs_GiaoDichID",
                        column: x => x.GiaoDichID,
                        principalTable: "LichSuGiaoDichs",
                        principalColumn: "GiaoDichID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuDoiGhes_NhanViens_NhanVienID",
                        column: x => x.NhanVienID,
                        principalTable: "NhanViens",
                        principalColumn: "NhanVienID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaoDuongs_XeID",
                table: "BaoDuongs",
                column: "XeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_VeXeVeID",
                table: "ChiTietVeDats",
                column: "VeXeVeID");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_NhanVienID",
                table: "HoaDons",
                column: "NhanVienID");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_VeXeVeID",
                table: "HoaDons",
                column: "VeXeVeID");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDoiGhes_ChiTietVeDatChiTietVeID",
                table: "LichSuDoiGhes",
                column: "ChiTietVeDatChiTietVeID");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDoiGhes_GiaoDichID",
                table: "LichSuDoiGhes",
                column: "GiaoDichID");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDoiGhes_NhanVienID",
                table: "LichSuDoiGhes",
                column: "NhanVienID");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuGiaoDichs_NhanVienID",
                table: "LichSuGiaoDichs",
                column: "NhanVienID");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuGiaoDichs_VeXeVeID",
                table: "LichSuGiaoDichs",
                column: "VeXeVeID");

            migrationBuilder.CreateIndex(
                name: "IX_LichTrinhs_TuyenDuongId",
                table: "LichTrinhs",
                column: "TuyenDuongId");

            migrationBuilder.CreateIndex(
                name: "IX_LichTrinhs_XeId",
                table: "LichTrinhs",
                column: "XeId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanXetKhachHangs_KhachHangID",
                table: "NhanXetKhachHangs",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_NhanXetKhachHangs_LichTrinhID",
                table: "NhanXetKhachHangs",
                column: "LichTrinhID");

            migrationBuilder.CreateIndex(
                name: "IX_VeXes_KhachHangID",
                table: "VeXes",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_VeXes_LichTrinhID",
                table: "VeXes",
                column: "LichTrinhID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoDuongs");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "LichSuDoiGhes");

            migrationBuilder.DropTable(
                name: "NhanXetKhachHangs");

            migrationBuilder.DropTable(
                name: "TaiXes");

            migrationBuilder.DropTable(
                name: "ChiTietVeDats");

            migrationBuilder.DropTable(
                name: "LichSuGiaoDichs");

            migrationBuilder.DropTable(
                name: "VeXes");

            migrationBuilder.DropTable(
                name: "LichTrinhs");

            migrationBuilder.DropTable(
                name: "TuyenDuongs");

            migrationBuilder.DropTable(
                name: "Xes");

            migrationBuilder.RenameColumn(
                name: "NhanVienID",
                table: "NhanViens",
                newName: "NhanVienId");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "NgayVaoLam",
                table: "NhanViens",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "NgaySinh",
                table: "NhanViens",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "TKId",
                table: "NhanViens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GioiTinh",
                table: "KhachHangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "NgaySinh",
                table: "KhachHangs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "TKId",
                table: "KhachHangs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_TKId",
                table: "NhanViens",
                column: "TKId");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_TKId",
                table: "KhachHangs",
                column: "TKId");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangs_TaiKhoans_TKId",
                table: "KhachHangs",
                column: "TKId",
                principalTable: "TaiKhoans",
                principalColumn: "TKId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NhanViens_TaiKhoans_TKId",
                table: "NhanViens",
                column: "TKId",
                principalTable: "TaiKhoans",
                principalColumn: "TKId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
