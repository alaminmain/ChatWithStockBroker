using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDividendInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DIVIDEND_INFO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COMP_CD = table.Column<int>(type: "int", nullable: true),
                    AGM_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FYEAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CFYEAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DIV_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RATE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RATIO1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RATIO2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PREMIUM = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PAYMENT_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BOK_CL_FDT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BOK_CL_TDT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OP_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DISCOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    REMARKS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BS_COMP_CD = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIVIDEND_INFO", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DIVIDEND_INFO");
        }
    }
}
