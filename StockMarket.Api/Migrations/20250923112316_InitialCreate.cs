using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "COMP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    COMP_CD = table.Column<int>(type: "int", nullable: true),
                    COMP_NM = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SECT_MAJ_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    SECT_MIN_CD = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    INSTR_CD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CAT_TP = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ADD1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ADD2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RegOff = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PRN_STH = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OPN_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TAX_HDAY = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    TEL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TLX = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    E_MAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                    PE_RATIO = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ISIN_CD = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    START_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LDRN = table.Column<int>(type: "int", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListingYear = table.Column<int>(type: "int", nullable: true),
                    LastAgmHeld = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EarningPerShare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetAssetValPerShare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NocfPerShare = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SharePercentageDirector = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SharePercentageForeign = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SharePercentageGovt = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SharePercentageInstitute = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SharePercentagePublic = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    YearEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OperationalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_COMP_SECT_MAJ_SECT_MAJ_CD",
                        column: x => x.SECT_MAJ_CD,
                        principalTable: "SECT_MAJ",
                        principalColumn: "SECT_MAJ_CD");
                });

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
                    table.ForeignKey(
                        name: "FK_DIVIDEND_INFO_COMP_COMP_CD",
                        column: x => x.COMP_CD,
                        principalTable: "COMP",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WatchLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchLists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WatchLists_COMP_CompId",
                        column: x => x.CompId,
                        principalTable: "COMP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_COMP_SECT_MAJ_CD",
                table: "COMP",
                column: "SECT_MAJ_CD");

            migrationBuilder.CreateIndex(
                name: "IX_DIVIDEND_INFO_COMP_CD",
                table: "DIVIDEND_INFO",
                column: "COMP_CD");

            migrationBuilder.CreateIndex(
                name: "IX_WatchLists_CompId",
                table: "WatchLists",
                column: "CompId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchLists_UserId",
                table: "WatchLists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DIVIDEND_INFO");

            migrationBuilder.DropTable(
                name: "MAR_PRICE");

            migrationBuilder.DropTable(
                name: "SECT_MIN");

            migrationBuilder.DropTable(
                name: "WatchLists");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "COMP");

            migrationBuilder.DropTable(
                name: "SECT_MAJ");
        }
    }
}
