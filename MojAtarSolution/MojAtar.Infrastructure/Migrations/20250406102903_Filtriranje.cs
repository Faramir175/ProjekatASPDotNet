using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Filtriranje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdKorisnik",
                table: "Resursi",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdKorisnik",
                table: "RadneMasine",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdKorisnik",
                table: "PrikljucneMasine",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdKorisnik",
                table: "Kulture",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdKorisnik",
                table: "Resursi");

            migrationBuilder.DropColumn(
                name: "IdKorisnik",
                table: "RadneMasine");

            migrationBuilder.DropColumn(
                name: "IdKorisnik",
                table: "PrikljucneMasine");

            migrationBuilder.DropColumn(
                name: "IdKorisnik",
                table: "Kulture");
        }
    }
}
