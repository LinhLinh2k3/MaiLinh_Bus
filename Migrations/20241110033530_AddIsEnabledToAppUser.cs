using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class AddIsEnabledToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NhanViens_AppUserId",
                table: "NhanViens");

            migrationBuilder.AddColumn<bool>(
                name: "isEnabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_AppUserId",
                table: "NhanViens",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NhanViens_AppUserId",
                table: "NhanViens");

            migrationBuilder.DropColumn(
                name: "isEnabled",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_AppUserId",
                table: "NhanViens",
                column: "AppUserId");
        }
    }
}
