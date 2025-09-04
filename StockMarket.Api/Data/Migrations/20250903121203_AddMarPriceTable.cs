using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarPriceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MAR_PRICE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TRANS_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    INST_CD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    COMP_CD = table.Column<int>(type: "int", nullable: true),
                    OPEN = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HIGH = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LOW = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CLOSE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CHG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VOL = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VAL = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GRP = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    MARK_TP = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    AVRG_RT = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    GEN_INDX = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    INDX_CHG = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MARK_CAP = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    T_VAL = table.Column<decimal>(type: "decimal(20,2)", nullable: true),
                    ISIN_CD = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DSEX_INDX = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MAR_PRICE", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MAR_PRICE");
        }
    }
}
