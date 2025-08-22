using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class ChinhSuaDB_AddSLGhe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SLGhe",
                table: "LoaiXe",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SLGhe",
                table: "LoaiXe");
        }
    }
}
