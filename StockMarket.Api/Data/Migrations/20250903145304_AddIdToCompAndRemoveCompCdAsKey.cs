using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdToCompAndRemoveCompCdAsKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_COMP",
                table: "COMP");

            // Drop the existing COMP_CD column
            migrationBuilder.DropColumn(
                name: "COMP_CD",
                table: "COMP");

            // Add COMP_CD column back as a regular column
            migrationBuilder.AddColumn<int>(
                name: "COMP_CD",
                table: "COMP",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "COMP",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_COMP",
                table: "COMP",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_COMP",
                table: "COMP");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "COMP");

            // Drop the non-identity COMP_CD column
            migrationBuilder.DropColumn(
                name: "COMP_CD",
                table: "COMP");

            // Add COMP_CD column back as an identity column
            migrationBuilder.AddColumn<int>(
                name: "COMP_CD",
                table: "COMP",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_COMP",
                table: "COMP",
                column: "COMP_CD");
        }
    }
}