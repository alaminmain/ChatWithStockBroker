using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeSectMajCdNullableInComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_COMP_SECT_MAJ_SECT_MAJ_CD",
                table: "COMP");

            migrationBuilder.AlterColumn<string>(
                name: "SECT_MAJ_CD",
                table: "COMP",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AddForeignKey(
                name: "FK_COMP_SECT_MAJ_SECT_MAJ_CD",
                table: "COMP",
                column: "SECT_MAJ_CD",
                principalTable: "SECT_MAJ",
                principalColumn: "SECT_MAJ_CD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_COMP_SECT_MAJ_SECT_MAJ_CD",
                table: "COMP");

            migrationBuilder.AlterColumn<string>(
                name: "SECT_MAJ_CD",
                table: "COMP",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_COMP_SECT_MAJ_SECT_MAJ_CD",
                table: "COMP",
                column: "SECT_MAJ_CD",
                principalTable: "SECT_MAJ",
                principalColumn: "SECT_MAJ_CD",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
