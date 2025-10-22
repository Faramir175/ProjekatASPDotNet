using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ParcelaKulturaDel_CascadeParcela_RestrictedKultura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture");

            migrationBuilder.DropForeignKey(
                name: "FK_ParceleKulture_Parcele_IdParcela",
                table: "ParceleKulture");

            migrationBuilder.AddForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParceleKulture_Parcele_IdParcela",
                table: "ParceleKulture",
                column: "IdParcela",
                principalTable: "Parcele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture");

            migrationBuilder.DropForeignKey(
                name: "FK_ParceleKulture_Parcele_IdParcela",
                table: "ParceleKulture");

            migrationBuilder.AddForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParceleKulture_Parcele_IdParcela",
                table: "ParceleKulture",
                column: "IdParcela",
                principalTable: "Parcele",
                principalColumn: "Id");
        }
    }
}
