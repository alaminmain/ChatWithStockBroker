using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToCompAndMarPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MAR_PRICE_COMP_CD_TRANS_DT",
                table: "MAR_PRICE",
                columns: new[] { "COMP_CD", "TRANS_DT" });

            migrationBuilder.CreateIndex(
                name: "IX_COMP_COMP_CD",
                table: "COMP",
                column: "COMP_CD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MAR_PRICE_COMP_CD_TRANS_DT",
                table: "MAR_PRICE");

            migrationBuilder.DropIndex(
                name: "IX_COMP_COMP_CD",
                table: "COMP");
        }
    }
}
