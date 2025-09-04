using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseRegOffLengthInComp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "REG_OFF",
                table: "COMP",
                newName: "RegOff");

            migrationBuilder.AlterColumn<string>(
                name: "RegOff",
                table: "COMP",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegOff",
                table: "COMP",
                newName: "REG_OFF");

            migrationBuilder.AlterColumn<string>(
                name: "REG_OFF",
                table: "COMP",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
