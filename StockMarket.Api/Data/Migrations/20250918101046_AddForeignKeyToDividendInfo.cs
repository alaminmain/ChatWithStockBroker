using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyToDividendInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DIVIDEND_INFO_COMP_CompId",
                table: "DIVIDEND_INFO");

            migrationBuilder.DropIndex(
                name: "IX_DIVIDEND_INFO_CompId",
                table: "DIVIDEND_INFO");

            migrationBuilder.DropColumn(
                name: "CompId",
                table: "DIVIDEND_INFO");

            migrationBuilder.CreateIndex(
                name: "IX_DIVIDEND_INFO_COMP_CD",
                table: "DIVIDEND_INFO",
                column: "COMP_CD");

            migrationBuilder.AddForeignKey(
                name: "FK_DIVIDEND_INFO_COMP_COMP_CD",
                table: "DIVIDEND_INFO",
                column: "COMP_CD",
                principalTable: "COMP",
                principalColumn: "Id");
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

            migrationBuilder.AddColumn<int>(
                name: "CompId",
                table: "DIVIDEND_INFO",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DIVIDEND_INFO_CompId",
                table: "DIVIDEND_INFO",
                column: "CompId");

            migrationBuilder.AddForeignKey(
                name: "FK_DIVIDEND_INFO_COMP_CompId",
                table: "DIVIDEND_INFO",
                column: "CompId",
                principalTable: "COMP",
                principalColumn: "Id");
        }
    }
}
