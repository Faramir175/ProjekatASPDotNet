using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRadnjaResursRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RadnjeResursi_Radnje_RadnjaId",
                table: "RadnjeResursi");

            migrationBuilder.DropIndex(
                name: "IX_RadnjeResursi_RadnjaId",
                table: "RadnjeResursi");

            migrationBuilder.DropColumn(
                name: "RadnjaId",
                table: "RadnjeResursi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RadnjaId",
                table: "RadnjeResursi",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RadnjeResursi_RadnjaId",
                table: "RadnjeResursi",
                column: "RadnjaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RadnjeResursi_Radnje_RadnjaId",
                table: "RadnjeResursi",
                column: "RadnjaId",
                principalTable: "Radnje",
                principalColumn: "Id");
        }
    }
}
