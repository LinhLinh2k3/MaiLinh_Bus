using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_VeXes_VeXeVeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_VeXes_VeXeVeID",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_LichSuGiaoDichs_VeXes_VeXeVeID",
                table: "LichSuGiaoDichs");

            migrationBuilder.DropIndex(
                name: "IX_LichSuGiaoDichs_VeXeVeID",
                table: "LichSuGiaoDichs");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_VeXeVeID",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_VeXeVeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "TrangThaiVe",
                table: "VeXes");

            migrationBuilder.DropColumn(
                name: "VeXeVeID",
                table: "LichSuGiaoDichs");

            migrationBuilder.DropColumn(
                name: "VeXeVeID",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "VeXeVeID",
                table: "ChiTietVeDats");

            migrationBuilder.RenameColumn(
                name: "SoGhe",
                table: "ChiTietVeDats",
                newName: "XeID");

            migrationBuilder.AlterColumn<string>(
                name: "VeID",
                table: "LichSuGiaoDichs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "VeID",
                table: "HoaDons",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "VeID",
                table: "ChiTietVeDats",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "GheID",
                table: "ChiTietVeDats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GheID1",
                table: "ChiTietVeDats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GheXeID",
                table: "ChiTietVeDats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LichSuGiaoDichs_VeID",
                table: "LichSuGiaoDichs",
                column: "VeID");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_VeID",
                table: "HoaDons",
                column: "VeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_GheID1_GheXeID",
                table: "ChiTietVeDats",
                columns: new[] { "GheID1", "GheXeID" });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_VeID",
                table: "ChiTietVeDats",
                column: "VeID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietVeDats_Ghes_GheID1_GheXeID",
                table: "ChiTietVeDats",
                columns: new[] { "GheID1", "GheXeID" },
                principalTable: "Ghes",
                principalColumns: new[] { "GheID", "XeID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietVeDats_VeXes_VeID",
                table: "ChiTietVeDats",
                column: "VeID",
                principalTable: "VeXes",
                principalColumn: "VeID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_VeXes_VeID",
                table: "HoaDons",
                column: "VeID",
                principalTable: "VeXes",
                principalColumn: "VeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuGiaoDichs_VeXes_VeID",
                table: "LichSuGiaoDichs",
                column: "VeID",
                principalTable: "VeXes",
                principalColumn: "VeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_Ghes_GheID1_GheXeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_VeXes_VeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_VeXes_VeID",
                table: "HoaDons");

            migrationBuilder.DropForeignKey(
                name: "FK_LichSuGiaoDichs_VeXes_VeID",
                table: "LichSuGiaoDichs");

            migrationBuilder.DropIndex(
                name: "IX_LichSuGiaoDichs_VeID",
                table: "LichSuGiaoDichs");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_VeID",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_GheID1_GheXeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_VeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "GheID",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "GheID1",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "GheXeID",
                table: "ChiTietVeDats");

            migrationBuilder.RenameColumn(
                name: "XeID",
                table: "ChiTietVeDats",
                newName: "SoGhe");

            migrationBuilder.AddColumn<string>(
                name: "TrangThaiVe",
                table: "VeXes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "VeID",
                table: "LichSuGiaoDichs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VeXeVeID",
                table: "LichSuGiaoDichs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "VeID",
                table: "HoaDons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VeXeVeID",
                table: "HoaDons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "VeID",
                table: "ChiTietVeDats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "VeXeVeID",
                table: "ChiTietVeDats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuGiaoDichs_VeXeVeID",
                table: "LichSuGiaoDichs",
                column: "VeXeVeID");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_VeXeVeID",
                table: "HoaDons",
                column: "VeXeVeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietVeDats_VeXeVeID",
                table: "ChiTietVeDats",
                column: "VeXeVeID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietVeDats_VeXes_VeXeVeID",
                table: "ChiTietVeDats",
                column: "VeXeVeID",
                principalTable: "VeXes",
                principalColumn: "VeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_VeXes_VeXeVeID",
                table: "HoaDons",
                column: "VeXeVeID",
                principalTable: "VeXes",
                principalColumn: "VeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LichSuGiaoDichs_VeXes_VeXeVeID",
                table: "LichSuGiaoDichs",
                column: "VeXeVeID",
                principalTable: "VeXes",
                principalColumn: "VeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
