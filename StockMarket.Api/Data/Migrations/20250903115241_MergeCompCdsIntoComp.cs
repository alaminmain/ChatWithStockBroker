using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class MergeCompCdsIntoComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COMP_CDS");

            migrationBuilder.AddColumn<string>(
                name: "ISIN_CD",
                table: "COMP",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LDRN",
                table: "COMP",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "START_DT",
                table: "COMP",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISIN_CD",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "LDRN",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "START_DT",
                table: "COMP");

            migrationBuilder.CreateTable(
                name: "COMP_CDS",
                columns: table => new
                {
                    COMP_CD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COMP_NM = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ISIN_CD = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    LDRN = table.Column<int>(type: "int", nullable: true),
                    START_DT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMP_CDS", x => x.COMP_CD);
                });
        }
    }
}
