using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class AddKhachHangTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangs_AspNetUsers_AppUserId",
                table: "KhachHangs");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangs_AppUserId",
                table: "KhachHangs");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "KhachHangs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_AppUserId",
                table: "KhachHangs",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangs_AspNetUsers_AppUserId",
                table: "KhachHangs",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KhachHangs_AspNetUsers_AppUserId",
                table: "KhachHangs");

            migrationBuilder.DropIndex(
                name: "IX_KhachHangs_AppUserId",
                table: "KhachHangs");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "KhachHangs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_AppUserId",
                table: "KhachHangs",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_KhachHangs_AspNetUsers_AppUserId",
                table: "KhachHangs",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
