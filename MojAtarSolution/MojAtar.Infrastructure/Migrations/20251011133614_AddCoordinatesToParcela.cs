using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatesToParcela : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Parcele",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Parcele",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Parcele");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Parcele");
        }
    }
}
