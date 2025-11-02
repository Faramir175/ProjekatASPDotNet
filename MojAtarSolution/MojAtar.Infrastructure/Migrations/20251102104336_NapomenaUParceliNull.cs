using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NapomenaUParceliNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Napomena",
                table: "Parcele",
                type: "nvarchar(175)",
                maxLength: 175,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(175)",
                oldMaxLength: 175);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Napomena",
                table: "Parcele",
                type: "nvarchar(175)",
                maxLength: 175,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(175)",
                oldMaxLength: 175,
                oldNullable: true);
        }
    }
}
