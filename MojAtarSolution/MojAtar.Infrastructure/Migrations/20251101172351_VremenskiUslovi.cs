using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VremenskiUslovi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VremenskiUslovi",
                table: "Radnje");

            migrationBuilder.AlterColumn<string>(
                name: "Napomena",
                table: "Radnje",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(175)",
                oldMaxLength: 175);

            migrationBuilder.AlterColumn<string>(
                name: "OpisServisa",
                table: "RadneMasine",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(175)",
                oldMaxLength: 175);

            migrationBuilder.AlterColumn<string>(
                name: "OpisServisa",
                table: "PrikljucneMasine",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(175)",
                oldMaxLength: 175);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Napomena",
                table: "Radnje",
                type: "nvarchar(175)",
                maxLength: 175,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VremenskiUslovi",
                table: "Radnje",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "OpisServisa",
                table: "RadneMasine",
                type: "nvarchar(175)",
                maxLength: 175,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "OpisServisa",
                table: "PrikljucneMasine",
                type: "nvarchar(175)",
                maxLength: 175,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
