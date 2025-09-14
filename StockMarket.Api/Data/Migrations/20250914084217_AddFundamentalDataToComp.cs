using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFundamentalDataToComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EarningPerShare",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "COMP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAgmHeld",
                table: "COMP",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ListingYear",
                table: "COMP",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetAssetValPerShare",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NocfPerShare",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationalStatus",
                table: "COMP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SharePercentageDirector",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SharePercentageForeign",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SharePercentageGovt",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SharePercentageInstitute",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SharePercentagePublic",
                table: "COMP",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "COMP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "YearEnd",
                table: "COMP",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EarningPerShare",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "LastAgmHeld",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "ListingYear",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "NetAssetValPerShare",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "NocfPerShare",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "OperationalStatus",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "SharePercentageDirector",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "SharePercentageForeign",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "SharePercentageGovt",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "SharePercentageInstitute",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "SharePercentagePublic",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "YearEnd",
                table: "COMP");
        }
    }
}
