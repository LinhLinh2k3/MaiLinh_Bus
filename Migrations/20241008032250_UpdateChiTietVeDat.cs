using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhaXeMaiLinh.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChiTietVeDat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_Ghes_GheID1_GheXeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_GheID1_GheXeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "GheID1",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "GheXeID",
                table: "ChiTietVeDats");

            migrationBuilder.AlterColumn<int>(
                name: "XeID",
                table: "ChiTietVeDats",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDat",
                table: "ChiTietVeDats",
                type: "datetime2",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietVeDats_Xes_XeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietVeDats_XeID",
                table: "ChiTietVeDats");

            migrationBuilder.DropColumn(
                name: "NgayDat",
                table: "ChiTietVeDats");

            migrationBuilder.AlterColumn<string>(
                name: "XeID",
                table: "ChiTietVeDats",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
                name: "IX_ChiTietVeDats_GheID1_GheXeID",
                table: "ChiTietVeDats",
                columns: new[] { "GheID1", "GheXeID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietVeDats_Ghes_GheID1_GheXeID",
                table: "ChiTietVeDats",
                columns: new[] { "GheID1", "GheXeID" },
                principalTable: "Ghes",
                principalColumns: new[] { "GheID", "XeID" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
