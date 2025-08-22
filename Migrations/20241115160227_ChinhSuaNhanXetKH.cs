using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class ChinhSuaNhanXetKH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NhanXetKhachHangs_LichTrinhs_LichTrinhID",
                table: "NhanXetKhachHangs");

            migrationBuilder.DropIndex(
                name: "IX_NhanXetKhachHangs_LichTrinhID",
                table: "NhanXetKhachHangs");

            migrationBuilder.AlterColumn<string>(
                name: "LichTrinhID",
                table: "NhanXetKhachHangs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LichTrinhID",
                table: "NhanXetKhachHangs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_NhanXetKhachHangs_LichTrinhID",
                table: "NhanXetKhachHangs",
                column: "LichTrinhID");

            migrationBuilder.AddForeignKey(
                name: "FK_NhanXetKhachHangs_LichTrinhs_LichTrinhID",
                table: "NhanXetKhachHangs",
                column: "LichTrinhID",
                principalTable: "LichTrinhs",
                principalColumn: "LichTrinhId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
