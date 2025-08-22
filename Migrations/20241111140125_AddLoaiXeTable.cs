using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class AddLoaiXeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Xes_LoaiXe_LoaiXeId",
                table: "Xes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoaiXe",
                table: "LoaiXe");

            migrationBuilder.RenameTable(
                name: "LoaiXe",
                newName: "LoaiXes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoaiXes",
                table: "LoaiXes",
                column: "LoaiXeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Xes_LoaiXes_LoaiXeId",
                table: "Xes",
                column: "LoaiXeId",
                principalTable: "LoaiXes",
                principalColumn: "LoaiXeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Xes_LoaiXes_LoaiXeId",
                table: "Xes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoaiXes",
                table: "LoaiXes");

            migrationBuilder.RenameTable(
                name: "LoaiXes",
                newName: "LoaiXe");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoaiXe",
                table: "LoaiXe",
                column: "LoaiXeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Xes_LoaiXe_LoaiXeId",
                table: "Xes",
                column: "LoaiXeId",
                principalTable: "LoaiXe",
                principalColumn: "LoaiXeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
