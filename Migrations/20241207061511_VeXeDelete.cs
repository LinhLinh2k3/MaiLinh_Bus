using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class VeXeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuGiaoDichs_NhanViens_NhanVienID",
                table: "LichSuGiaoDichs");

            migrationBuilder.AddColumn<bool>(
                name: "isDelete",
                table: "VeXes",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NhanVienID",
                table: "LichSuGiaoDichs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuGiaoDichs_NhanViens_NhanVienID",
                table: "LichSuGiaoDichs",
                column: "NhanVienID",
                principalTable: "NhanViens",
                principalColumn: "NhanVienID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichSuGiaoDichs_NhanViens_NhanVienID",
                table: "LichSuGiaoDichs");

            migrationBuilder.DropColumn(
                name: "isDelete",
                table: "VeXes");

            migrationBuilder.AlterColumn<string>(
                name: "NhanVienID",
                table: "LichSuGiaoDichs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuGiaoDichs_NhanViens_NhanVienID",
                table: "LichSuGiaoDichs",
                column: "NhanVienID",
                principalTable: "NhanViens",
                principalColumn: "NhanVienID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
