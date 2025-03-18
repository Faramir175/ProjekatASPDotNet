using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KatastarskeOpstine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    GradskaOpstina = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KatastarskeOpstine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Uloga = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DatumRegistracije = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lozinka = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kulture",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Hibrid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AktuelnaCena = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kulture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrikljucneMasine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipMasine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SirinaObrade = table.Column<double>(type: "float", nullable: false),
                    PoslednjiServis = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpisServisa = table.Column<string>(type: "nvarchar(175)", maxLength: 175, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrikljucneMasine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RadneMasine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TipUlja = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    RadniSatiServis = table.Column<int>(type: "int", nullable: false),
                    PoslednjiServis = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpisServisa = table.Column<string>(type: "nvarchar(175)", maxLength: 175, nullable: false),
                    UkupanBrojRadnihSati = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadneMasine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Resursi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Vrsta = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    AktuelnaCena = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resursi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcele",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrojParcele = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Povrsina = table.Column<double>(type: "float", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(175)", maxLength: 175, nullable: false),
                    IdKatastarskaOpstina = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdKorisnik = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KorisnikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcele", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parcele_KatastarskeOpstine_IdKatastarskaOpstina",
                        column: x => x.IdKatastarskaOpstina,
                        principalTable: "KatastarskeOpstine",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Parcele_Korisnici_KorisnikId",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CeneKultura",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdKultura = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KulturaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenaPojedinici = table.Column<double>(type: "float", nullable: false),
                    DatumVaznosti = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CeneKultura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CeneKultura_Kulture_KulturaId",
                        column: x => x.KulturaId,
                        principalTable: "Kulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CeneResursa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdResurs = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResursId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenaPojedinici = table.Column<double>(type: "float", nullable: false),
                    DatumVaznosti = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CeneResursa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CeneResursa_Resursi_ResursId",
                        column: x => x.ResursId,
                        principalTable: "Resursi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParceleKulture",
                columns: table => new
                {
                    IdParcela = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdKultura = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Povrsina = table.Column<double>(type: "float", nullable: false),
                    DatumSetve = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumZetve = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParceleKulture", x => new { x.IdParcela, x.IdKultura });
                    table.ForeignKey(
                        name: "FK_ParceleKulture_Kulture_IdKultura",
                        column: x => x.IdKultura,
                        principalTable: "Kulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParceleKulture_Parcele_IdParcela",
                        column: x => x.IdParcela,
                        principalTable: "Parcele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Radnje",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdParcela = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParcelaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipRadnje = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DatumIzvrsenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VremenskiUslovi = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(175)", maxLength: 175, nullable: false),
                    UkupanTrosak = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Radnje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Radnje_Parcele_ParcelaId",
                        column: x => x.ParcelaId,
                        principalTable: "Parcele",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadnjePrikljucneMasine",
                columns: table => new
                {
                    IdRadnja = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdPrikljucnaMasina = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrojRadnihSati = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadnjePrikljucneMasine", x => new { x.IdRadnja, x.IdPrikljucnaMasina });
                    table.ForeignKey(
                        name: "FK_RadnjePrikljucneMasine_PrikljucneMasine_IdPrikljucnaMasina",
                        column: x => x.IdPrikljucnaMasina,
                        principalTable: "PrikljucneMasine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RadnjePrikljucneMasine_Radnje_IdRadnja",
                        column: x => x.IdRadnja,
                        principalTable: "Radnje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadnjeRadneMasine",
                columns: table => new
                {
                    IdRadnja = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdRadnaMasina = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrojRadnihSati = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadnjeRadneMasine", x => new { x.IdRadnja, x.IdRadnaMasina });
                    table.ForeignKey(
                        name: "FK_RadnjeRadneMasine_RadneMasine_IdRadnaMasina",
                        column: x => x.IdRadnaMasina,
                        principalTable: "RadneMasine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RadnjeRadneMasine_Radnje_IdRadnja",
                        column: x => x.IdRadnja,
                        principalTable: "Radnje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RadnjeResursi",
                columns: table => new
                {
                    IdRadnja = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdResurs = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kolicina = table.Column<double>(type: "float", nullable: false),
                    DatumKoriscenja = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadnjeResursi", x => new { x.IdRadnja, x.IdResurs });
                    table.ForeignKey(
                        name: "FK_RadnjeResursi_Radnje_IdRadnja",
                        column: x => x.IdRadnja,
                        principalTable: "Radnje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RadnjeResursi_Resursi_IdResurs",
                        column: x => x.IdResurs,
                        principalTable: "Resursi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zetva",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdKultura = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KulturaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prinos = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zetva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zetva_Kulture_KulturaId",
                        column: x => x.KulturaId,
                        principalTable: "Kulture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zetva_Radnje_Id",
                        column: x => x.Id,
                        principalTable: "Radnje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CeneKultura_KulturaId",
                table: "CeneKultura",
                column: "KulturaId");

            migrationBuilder.CreateIndex(
                name: "IX_CeneResursa_ResursId",
                table: "CeneResursa",
                column: "ResursId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcele_IdKatastarskaOpstina",
                table: "Parcele",
                column: "IdKatastarskaOpstina");

            migrationBuilder.CreateIndex(
                name: "IX_Parcele_KorisnikId",
                table: "Parcele",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ParceleKulture_IdKultura",
                table: "ParceleKulture",
                column: "IdKultura");

            migrationBuilder.CreateIndex(
                name: "IX_Radnje_ParcelaId",
                table: "Radnje",
                column: "ParcelaId");

            migrationBuilder.CreateIndex(
                name: "IX_RadnjePrikljucneMasine_IdPrikljucnaMasina",
                table: "RadnjePrikljucneMasine",
                column: "IdPrikljucnaMasina");

            migrationBuilder.CreateIndex(
                name: "IX_RadnjeRadneMasine_IdRadnaMasina",
                table: "RadnjeRadneMasine",
                column: "IdRadnaMasina");

            migrationBuilder.CreateIndex(
                name: "IX_RadnjeResursi_IdResurs",
                table: "RadnjeResursi",
                column: "IdResurs");

            migrationBuilder.CreateIndex(
                name: "IX_Zetva_KulturaId",
                table: "Zetva",
                column: "KulturaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CeneKultura");

            migrationBuilder.DropTable(
                name: "CeneResursa");

            migrationBuilder.DropTable(
                name: "ParceleKulture");

            migrationBuilder.DropTable(
                name: "RadnjePrikljucneMasine");

            migrationBuilder.DropTable(
                name: "RadnjeRadneMasine");

            migrationBuilder.DropTable(
                name: "RadnjeResursi");

            migrationBuilder.DropTable(
                name: "Zetva");

            migrationBuilder.DropTable(
                name: "PrikljucneMasine");

            migrationBuilder.DropTable(
                name: "RadneMasine");

            migrationBuilder.DropTable(
                name: "Resursi");

            migrationBuilder.DropTable(
                name: "Kulture");

            migrationBuilder.DropTable(
                name: "Radnje");

            migrationBuilder.DropTable(
                name: "Parcele");

            migrationBuilder.DropTable(
                name: "KatastarskeOpstine");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
