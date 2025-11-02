using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StrukturnaOgranicenjaDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CeneKultura_Kulture_IdKultura",
                table: "CeneKultura");

            migrationBuilder.DropForeignKey(
                name: "FK_CeneResursa_Resursi_IdResurs",
                table: "CeneResursa");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcele_KatastarskeOpstine_IdKatastarskaOpstina",
                table: "Parcele");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcele_Korisnici_IdKorisnik",
                table: "Parcele");

            migrationBuilder.DropForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture");

            migrationBuilder.DropForeignKey(
                name: "FK_Radnje_Kulture_IdKultura",
                table: "Radnje");

            migrationBuilder.DropForeignKey(
                name: "FK_Radnje_Parcele_IdParcela",
                table: "Radnje");

            migrationBuilder.AddForeignKey(
                name: "FK_CeneKultura_Kulture_IdKultura",
                table: "CeneKultura",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CeneResursa_Resursi_IdResurs",
                table: "CeneResursa",
                column: "IdResurs",
                principalTable: "Resursi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parcele_KatastarskeOpstine_IdKatastarskaOpstina",
                table: "Parcele",
                column: "IdKatastarskaOpstina",
                principalTable: "KatastarskeOpstine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parcele_Korisnici_IdKorisnik",
                table: "Parcele",
                column: "IdKorisnik",
                principalTable: "Korisnici",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Radnje_Kulture_IdKultura",
                table: "Radnje",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Radnje_Parcele_IdParcela",
                table: "Radnje",
                column: "IdParcela",
                principalTable: "Parcele",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CeneKultura_Kulture_IdKultura",
                table: "CeneKultura");

            migrationBuilder.DropForeignKey(
                name: "FK_CeneResursa_Resursi_IdResurs",
                table: "CeneResursa");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcele_KatastarskeOpstine_IdKatastarskaOpstina",
                table: "Parcele");

            migrationBuilder.DropForeignKey(
                name: "FK_Parcele_Korisnici_IdKorisnik",
                table: "Parcele");

            migrationBuilder.DropForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture");

            migrationBuilder.DropForeignKey(
                name: "FK_Radnje_Kulture_IdKultura",
                table: "Radnje");

            migrationBuilder.DropForeignKey(
                name: "FK_Radnje_Parcele_IdParcela",
                table: "Radnje");

            migrationBuilder.AddForeignKey(
                name: "FK_CeneKultura_Kulture_IdKultura",
                table: "CeneKultura",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CeneResursa_Resursi_IdResurs",
                table: "CeneResursa",
                column: "IdResurs",
                principalTable: "Resursi",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcele_KatastarskeOpstine_IdKatastarskaOpstina",
                table: "Parcele",
                column: "IdKatastarskaOpstina",
                principalTable: "KatastarskeOpstine",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Parcele_Korisnici_IdKorisnik",
                table: "Parcele",
                column: "IdKorisnik",
                principalTable: "Korisnici",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParceleKulture_Kulture_IdKultura",
                table: "ParceleKulture",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Radnje_Kulture_IdKultura",
                table: "Radnje",
                column: "IdKultura",
                principalTable: "Kulture",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Radnje_Parcele_IdParcela",
                table: "Radnje",
                column: "IdParcela",
                principalTable: "Parcele",
                principalColumn: "Id");
        }
    }
}
