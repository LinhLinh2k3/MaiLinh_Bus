using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class capnhatdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_Xes_XeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_VeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_XeID",
                table: "ChiTietVeDats");

            migrationBuilder.RenameColumn(
                name: "GheID",
                table: "ChiTietVeDats",
                newName: "SoGhe");

            migrationBuilder.AlterColumn<int>(
                name: "GheMoi",
                table: "LichSuDoiGhes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "GheCu",
                table: "LichSuDoiGhes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_SoGhe_XeID",
                table: "ChiTietVeDats",
                columns: new[] { "SoGhe", "XeID" });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_VeID",
                table: "ChiTietVeDats",
                column: "VeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietVeDats_Ghes_SoGhe_XeID",
                table: "ChiTietVeDats",
                columns: new[] { "SoGhe", "XeID" },
                principalTable: "Ghes",
                principalColumns: new[] { "GheID", "XeID" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_Ghes_SoGhe_XeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_SoGhe_XeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_VeID",
                table: "ChiTietVeDats");

            migrationBuilder.RenameColumn(
                name: "SoGhe",
                table: "ChiTietVeDats",
                newName: "GheID");

            migrationBuilder.AlterColumn<string>(
                name: "GheMoi",
                table: "LichSuDoiGhes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GheCu",
                table: "LichSuDoiGhes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_VeID",
                table: "ChiTietVeDats",
                column: "VeID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_XeID",
                table: "ChiTietVeDats",
                column: "XeID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietVeDats_Xes_XeID",
                table: "ChiTietVeDats",
                column: "XeID",
                principalTable: "Xes",
                principalColumn: "XeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
