using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DodataVezaRadnjaRadnaMasina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RadnjaId",
                table: "RadnjeResursi",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RadnjaId",
                table: "RadnjePrikljucneMasine",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadnjeResursi_RadnjaId",
                table: "RadnjeResursi",
                column: "RadnjaId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_RadnjeResursi_Radnje_RadnjaId",
                table: "RadnjeResursi",
                column: "RadnjaId",
                principalTable: "Radnje",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadnjePrikljucneMasine_Radnje_RadnjaId",
                table: "RadnjePrikljucneMasine");

            migrationBuilder.DropForeignKey(
                name: "FK_RadnjeResursi_Radnje_RadnjaId",
                table: "RadnjeResursi");

            migrationBuilder.DropIndex(
                name: "IX_RadnjeResursi_RadnjaId",
                table: "RadnjeResursi");

            migrationBuilder.DropIndex(
                name: "IX_RadnjePrikljucneMasine_RadnjaId",
                table: "RadnjePrikljucneMasine");

            migrationBuilder.DropColumn(
                name: "RadnjaId",
                table: "RadnjeResursi");

            migrationBuilder.DropColumn(
                name: "RadnjaId",
                table: "RadnjePrikljucneMasine");
        }
    }
}
