using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRadnjaParcelaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Radnje_Parcele_IdParcela",
                table: "Radnje");

            migrationBuilder.DropIndex(
                name: "IX_Radnje_IdParcela",
                table: "Radnje");

            migrationBuilder.DropColumn(
                name: "IdParcela",
                table: "Radnje");

            migrationBuilder.CreateTable(
                name: "RadnjeParcele",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRadnja = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdParcela = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Povrsina = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadnjeParcele", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadnjeParcele_Parcele_IdParcela",
                        column: x => x.IdParcela,
                        principalTable: "Parcele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RadnjeParcele_Radnje_IdRadnja",
                        column: x => x.IdRadnja,
                        principalTable: "Radnje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadnjeParcele_IdParcela",
                table: "RadnjeParcele",
                column: "IdParcela");

            migrationBuilder.CreateIndex(
                name: "IX_RadnjeParcele_IdRadnja",
                table: "RadnjeParcele",
                column: "IdRadnja");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RadnjeParcele");

            migrationBuilder.AddColumn<Guid>(
                name: "IdParcela",
                table: "Radnje",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Radnje_IdParcela",
                table: "Radnje",
                column: "IdParcela");

            migrationBuilder.AddForeignKey(
                name: "FK_Radnje_Parcele_IdParcela",
                table: "Radnje",
                column: "IdParcela",
                principalTable: "Parcele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
