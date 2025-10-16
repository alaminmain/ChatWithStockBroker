using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    StatementType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialReports_COMP_CompId",
                        column: x => x.CompId,
                        principalTable: "COMP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialReportEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinancialReportId = table.Column<int>(type: "int", nullable: false),
                    StandardAccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalAccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReportEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialReportEntries_FinancialReports_FinancialReportId",
                        column: x => x.FinancialReportId,
                        principalTable: "FinancialReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReportEntries_FinancialReportId",
                table: "FinancialReportEntries",
                column: "FinancialReportId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_CompId",
                table: "FinancialReports",
                column: "CompId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialReportEntries");

            migrationBuilder.DropTable(
                name: "FinancialReports");
        }
    }
}
