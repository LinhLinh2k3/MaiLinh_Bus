using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class ThemKhuyenMai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KhuyenMaiID",
                table: "VeXes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "KhuyenMais",
                columns: table => new
                {
                    KhuyenMaiID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenKhuyenMai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiKhuyenMai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DieuKienApDung = table.Column<int>(type: "int", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMais", x => x.KhuyenMaiID);
                });

            migrationBuilder.CreateTable(
                name: "KhuyenMai_KHcs",
                columns: table => new
                {
                    KhuyenMaiID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KhachHangID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai_KHcs", x => new { x.KhuyenMaiID, x.KhachHangID });
                    table.ForeignKey(
                        name: "FK_KhuyenMai_KHcs_KhachHangs_KhachHangID",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KhuyenMai_KHcs_KhuyenMai_KhuyenMaiID",
                        column: x => x.KhuyenMaiID,
                        principalTable: "KhuyenMais",
                        principalColumn: "KhuyenMaiID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VeXes_KhuyenMaiID",
                table: "VeXes",
                column: "KhuyenMaiID");

            migrationBuilder.CreateIndex(
                name: "IX_KhuyenMai_KHcs_KhachHangID",
                table: "KhuyenMai_KHcs",
                column: "KhachHangID");

            migrationBuilder.AddForeignKey(
                name: "FK_VeXes_KhuyenMai_KhuyenMaiID",
                table: "VeXes",
                column: "KhuyenMaiID",
                principalTable: "KhuyenMais",
                principalColumn: "KhuyenMaiID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VeXes_KhuyenMai_KhuyenMaiID",
                table: "VeXes");

            migrationBuilder.DropTable(
                name: "KhuyenMai_KHcs");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropIndex(
                name: "IX_VeXes_KhuyenMaiID",
                table: "VeXes");

            migrationBuilder.DropColumn(
                name: "KhuyenMaiID",
                table: "VeXes");
        }
    }
}
