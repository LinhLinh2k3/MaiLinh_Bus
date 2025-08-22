using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class HoanThanhDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaoDuongs");

            migrationBuilder.DropColumn(
                name: "BaoDuong",
                table: "Xes");

            migrationBuilder.DropColumn(
                name: "LoaiXe",
                table: "Xes");

            migrationBuilder.DropColumn(
                name: "TuyenXe",
                table: "LichTrinhs");

            migrationBuilder.AddColumn<string>(
                name: "LoaiXeId",
                table: "Xes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "GioKhoiHanh",
                table: "LichTrinhs",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "GioDen",
                table: "LichTrinhs",
                type: "time",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "DieuChinhGiaVe",
                table: "LichTrinhs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "GiaVe",
                table: "LichTrinhs",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NgayDen",
                table: "LichTrinhs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "NgayKhoiHanh",
                table: "LichTrinhs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "Ghes",
                columns: table => new
                {
                    GheID = table.Column<int>(type: "int", nullable: false),
                    XeID = table.Column<int>(type: "int", nullable: false),
                    TenGhe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ghes", x => new { x.GheID, x.XeID });
                    table.ForeignKey(
                        name: "FK_Ghes_Xes_XeID",
                        column: x => x.XeID,
                        principalTable: "Xes",
                        principalColumn: "XeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoaiXe",
                columns: table => new
                {
                    LoaiXeID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenLoaiXe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HangXe = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiXe", x => x.LoaiXeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Xes_LoaiXeId",
                table: "Xes",
                column: "LoaiXeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ghes_XeID",
                table: "Ghes",
                column: "XeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Xes_LoaiXe_LoaiXeId",
                table: "Xes",
                column: "LoaiXeId",
                principalTable: "LoaiXe",
                principalColumn: "LoaiXeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Xes_LoaiXe_LoaiXeId",
                table: "Xes");

            migrationBuilder.DropTable(
                name: "Ghes");

            migrationBuilder.DropTable(
                name: "LoaiXe");

            migrationBuilder.DropIndex(
                name: "IX_Xes_LoaiXeId",
                table: "Xes");

            migrationBuilder.DropColumn(
                name: "LoaiXeId",
                table: "Xes");

            migrationBuilder.DropColumn(
                name: "DieuChinhGiaVe",
                table: "LichTrinhs");

            migrationBuilder.DropColumn(
                name: "GiaVe",
                table: "LichTrinhs");

            migrationBuilder.DropColumn(
                name: "NgayDen",
                table: "LichTrinhs");

            migrationBuilder.DropColumn(
                name: "NgayKhoiHanh",
                table: "LichTrinhs");

            migrationBuilder.AddColumn<DateTime>(
                name: "BaoDuong",
                table: "Xes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LoaiXe",
                table: "Xes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GioKhoiHanh",
                table: "LichTrinhs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GioDen",
                table: "LichTrinhs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<string>(
                name: "TuyenXe",
                table: "LichTrinhs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BaoDuongs",
                columns: table => new
                {
                    BaoDuongID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    XeID = table.Column<int>(type: "int", nullable: false),
                    ChiPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoaiBaoDuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTaSuCo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayBaoDuong = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_BaoDuongs_XeID",
                table: "BaoDuongs",
                column: "XeID");
        }
    }
}
