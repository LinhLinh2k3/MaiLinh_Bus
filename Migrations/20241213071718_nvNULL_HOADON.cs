using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class nvNULL_HOADON : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienID",
                table: "HoaDons");

            migrationBuilder.AlterColumn<string>(
                name: "NhanVienID",
                table: "HoaDons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienID",
                table: "HoaDons",
                column: "NhanVienID",
                principalTable: "NhanViens",
                principalColumn: "NhanVienID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienID",
                table: "HoaDons");

            migrationBuilder.AlterColumn<string>(
                name: "NhanVienID",
                table: "HoaDons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_NhanViens_NhanVienID",
                table: "HoaDons",
                column: "NhanVienID",
                principalTable: "NhanViens",
                principalColumn: "NhanVienID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
