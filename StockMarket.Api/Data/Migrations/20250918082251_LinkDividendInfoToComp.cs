using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkDividendInfoToComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateIndex(
            //    name: "IX_COMP_COMP_CD",
            //    table: "COMP",
            //    column: "COMP_CD",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_DIVIDEND_INFO_COMP_CD",
            //    table: "DIVIDEND_INFO",
            //    column: "COMP_CD");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_DIVIDEND_INFO_COMP_COMP_CD",
            //    table: "DIVIDEND_INFO",
            //    column: "COMP_CD",
            //    principalTable: "COMP",
            //    principalColumn: "COMP_CD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DIVIDEND_INFO_COMP_COMP_CD",
                table: "DIVIDEND_INFO");

            migrationBuilder.DropIndex(
                name: "IX_DIVIDEND_INFO_COMP_CD",
                table: "DIVIDEND_INFO");

            migrationBuilder.DropIndex(
                name: "IX_COMP_COMP_CD",
                table: "COMP");
        }
    }
}
