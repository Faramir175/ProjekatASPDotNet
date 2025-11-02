using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IspravkaPriklkjucneMasine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RadnjaId",
                table: "RadnjePrikljucneMasine",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadnjePrikljucneMasine_RadnjaId",
                table: "RadnjePrikljucneMasine",
                column: "RadnjaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RadnjePrikljucneMasine_Radnje_RadnjaId",
                table: "RadnjePrikljucneMasine",
                column: "RadnjaId",
                principalTable: "Radnje",
                principalColumn: "Id");
        }
    }
}
