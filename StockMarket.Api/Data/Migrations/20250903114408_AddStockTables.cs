using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStockTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COMP_CDS",
                columns: table => new
                {
                    COMP_CD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COMP_NM = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ISIN_CD = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    START_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LDRN = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMP_CDS", x => x.COMP_CD);
                });

            migrationBuilder.CreateTable(
                name: "SECT_MAJ",
                columns: table => new
                {
                    SECT_MAJ_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    SECT_MAJ_NM = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECT_MAJ", x => x.SECT_MAJ_CD);
                });

            migrationBuilder.CreateTable(
                name: "SECT_MIN",
                columns: table => new
                {
                    SECT_MIN_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    SECT_MAJ_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    SECT_MIN_NM = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECT_MIN", x => x.SECT_MIN_CD);
                });

            migrationBuilder.CreateTable(
                name: "COMP",
                columns: table => new
                {
                    COMP_CD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COMP_NM = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SECT_MAJ_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    SECT_MIN_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    INSTR_CD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CAT_TP = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ADD1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ADD2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    REG_OFF = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PRN_STH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OPN_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TAX_HDAY = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    TEL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TLX = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    E_MAIL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PROD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PRO_VOL = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SPNR = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ATHO_CAP = table.Column<decimal>(type: "decimal(17,2)", nullable: true),
                    PAID_CAP = table.Column<decimal>(type: "decimal(17,2)", nullable: false),
                    NO_SHRS = table.Column<decimal>(type: "decimal(17,2)", nullable: false),
                    FC_VAL = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    MLOT = table.Column<int>(type: "int", nullable: false),
                    SBASE_RT = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    FLOT_DT_FM = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FLOT_DT_TO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BOK_CL_FDT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BOK_CL_TDT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MARGIN = table.Column<int>(type: "int", nullable: true),
                    AVG_RT = table.Column<decimal>(type: "decimal(12,4)", nullable: true),
                    RT_UPD_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FLAG = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    AUDITOR = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    NS_ICB = table.Column<decimal>(type: "decimal(17,2)", nullable: true),
                    NS_UNIT = table.Column<decimal>(type: "decimal(17,2)", nullable: true),
                    NS_MUTUAL = table.Column<decimal>(type: "decimal(17,2)", nullable: true),
                    PMARGIN = table.Column<int>(type: "int", nullable: true),
                    RISSU_DT_FM = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RISSU_DT_TO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PREMIUM = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    CFLAG = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    MAR_FLOAT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MON_TO = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    TRADE_METH = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CSEINSTR_CD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    INDX_LST = table.Column<decimal>(type: "decimal(13,4)", nullable: true),
                    BASE_UPD_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CDS = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CTL_RT = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    NET = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    GRP = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    MERCHAN_BANK_ID = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    OTC = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    IPO_CUTOFF_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TRADE_PLATFORM = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    PE_RATIO = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMP", x => x.COMP_CD);
                    table.ForeignKey(
                        name: "FK_COMP_SECT_MAJ_SECT_MAJ_CD",
                        column: x => x.SECT_MAJ_CD,
                        principalTable: "SECT_MAJ",
                        principalColumn: "SECT_MAJ_CD",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_COMP_SECT_MAJ_CD",
                table: "COMP",
                column: "SECT_MAJ_CD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "COMP");

            migrationBuilder.DropTable(
                name: "COMP_CDS");

            migrationBuilder.DropTable(
                name: "SECT_MIN");

            migrationBuilder.DropTable(
                name: "SECT_MAJ");
        }
    }
}
