﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MojAtar.Infrastructure.MojAtar;

#nullable disable

namespace MojAtar.Infrastructure.Migrations
{
    [DbContext(typeof(MojAtarDbContext))]
    [Migration("20250318213149_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MojAtar.Core.Domain.CenaKulture", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("CenaPojedinici")
                        .HasColumnType("float");

                    b.Property<DateTime>("DatumVaznosti")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("IdKultura")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("KulturaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("KulturaId");

                    b.ToTable("CeneKultura");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.CenaResursa", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("CenaPojedinici")
                        .HasColumnType("float");

                    b.Property<DateTime>("DatumVaznosti")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("IdResurs")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ResursId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ResursId");

                    b.ToTable("CeneResursa");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.KatastarskaOpstina", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("GradskaOpstina")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("KatastarskeOpstine");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Korisnik", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DatumRegistracije")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Ime")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Lozinka")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Prezime")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Uloga")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Korisnici");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Kultura", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("AktuelnaCena")
                        .HasColumnType("float");

                    b.Property<string>("Hibrid")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Kulture");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Parcela", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BrojParcele")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<Guid?>("IdKatastarskaOpstina")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("IdKorisnik")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("KorisnikId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Napomena")
                        .IsRequired()
                        .HasMaxLength(175)
                        .HasColumnType("nvarchar(175)");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<double>("Povrsina")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("IdKatastarskaOpstina");

                    b.HasIndex("KorisnikId");

                    b.ToTable("Parcele");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Parcela_Kultura", b =>
                {
                    b.Property<Guid>("IdParcela")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdKultura")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DatumSetve")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DatumZetve")
                        .HasColumnType("datetime2");

                    b.Property<double>("Povrsina")
                        .HasColumnType("float");

                    b.HasKey("IdParcela", "IdKultura");

                    b.HasIndex("IdKultura");

                    b.ToTable("ParceleKulture");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.PrikljucnaMasina", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("OpisServisa")
                        .IsRequired()
                        .HasMaxLength(175)
                        .HasColumnType("nvarchar(175)");

                    b.Property<DateTime>("PoslednjiServis")
                        .HasColumnType("datetime2");

                    b.Property<double>("SirinaObrade")
                        .HasColumnType("float");

                    b.Property<string>("TipMasine")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PrikljucneMasine");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.RadnaMasina", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("OpisServisa")
                        .IsRequired()
                        .HasMaxLength(175)
                        .HasColumnType("nvarchar(175)");

                    b.Property<DateTime>("PoslednjiServis")
                        .HasColumnType("datetime2");

                    b.Property<int>("RadniSatiServis")
                        .HasColumnType("int");

                    b.Property<string>("TipUlja")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<int>("UkupanBrojRadnihSati")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("RadneMasine");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DatumIzvrsenja")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("IdParcela")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Napomena")
                        .IsRequired()
                        .HasMaxLength(175)
                        .HasColumnType("nvarchar(175)");

                    b.Property<Guid>("ParcelaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TipRadnje")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<double>("UkupanTrosak")
                        .HasColumnType("float");

                    b.Property<string>("VremenskiUslovi")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.HasIndex("ParcelaId");

                    b.ToTable("Radnje");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja_PrikljucnaMasina", b =>
                {
                    b.Property<Guid>("IdRadnja")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdPrikljucnaMasina")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BrojRadnihSati")
                        .HasColumnType("int");

                    b.HasKey("IdRadnja", "IdPrikljucnaMasina");

                    b.HasIndex("IdPrikljucnaMasina");

                    b.ToTable("RadnjePrikljucneMasine");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja_RadnaMasina", b =>
                {
                    b.Property<Guid>("IdRadnja")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdRadnaMasina")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("BrojRadnihSati")
                        .HasColumnType("int");

                    b.HasKey("IdRadnja", "IdRadnaMasina");

                    b.HasIndex("IdRadnaMasina");

                    b.ToTable("RadnjeRadneMasine");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja_Resurs", b =>
                {
                    b.Property<Guid>("IdRadnja")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdResurs")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DatumKoriscenja")
                        .HasColumnType("datetime2");

                    b.Property<double>("Kolicina")
                        .HasColumnType("float");

                    b.HasKey("IdRadnja", "IdResurs");

                    b.HasIndex("IdResurs");

                    b.ToTable("RadnjeResursi");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Resurs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("AktuelnaCena")
                        .HasColumnType("float");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Vrsta")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("Id");

                    b.ToTable("Resursi");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Zetva", b =>
                {
                    b.HasBaseType("MojAtar.Core.Domain.Radnja");

                    b.Property<Guid?>("IdKultura")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("KulturaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Prinos")
                        .HasColumnType("float");

                    b.HasIndex("KulturaId");

                    b.ToTable("Zetva", (string)null);
                });

            modelBuilder.Entity("MojAtar.Core.Domain.CenaKulture", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.Kultura", "Kultura")
                        .WithMany()
                        .HasForeignKey("KulturaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kultura");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.CenaResursa", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.Resurs", "Resurs")
                        .WithMany()
                        .HasForeignKey("ResursId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resurs");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Parcela", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.KatastarskaOpstina", "KatastarskaOpstina")
                        .WithMany("Parcele")
                        .HasForeignKey("IdKatastarskaOpstina");

                    b.HasOne("MojAtar.Core.Domain.Korisnik", "Korisnik")
                        .WithMany("Parcele")
                        .HasForeignKey("KorisnikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KatastarskaOpstina");

                    b.Navigation("Korisnik");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Parcela_Kultura", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.Kultura", "Kultura")
                        .WithMany("ParceleKulture")
                        .HasForeignKey("IdKultura")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MojAtar.Core.Domain.Parcela", "Parcela")
                        .WithMany("ParceleKulture")
                        .HasForeignKey("IdParcela")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kultura");

                    b.Navigation("Parcela");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.Parcela", "Parcela")
                        .WithMany("Radnje")
                        .HasForeignKey("ParcelaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Parcela");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja_PrikljucnaMasina", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.PrikljucnaMasina", "PrikljucnaMasina")
                        .WithMany()
                        .HasForeignKey("IdPrikljucnaMasina")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MojAtar.Core.Domain.Radnja", "Radnja")
                        .WithMany()
                        .HasForeignKey("IdRadnja")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PrikljucnaMasina");

                    b.Navigation("Radnja");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja_RadnaMasina", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.RadnaMasina", "RadnaMasina")
                        .WithMany()
                        .HasForeignKey("IdRadnaMasina")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MojAtar.Core.Domain.Radnja", "Radnja")
                        .WithMany()
                        .HasForeignKey("IdRadnja")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RadnaMasina");

                    b.Navigation("Radnja");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Radnja_Resurs", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.Radnja", "Radnja")
                        .WithMany()
                        .HasForeignKey("IdRadnja")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MojAtar.Core.Domain.Resurs", "Resurs")
                        .WithMany()
                        .HasForeignKey("IdResurs")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Radnja");

                    b.Navigation("Resurs");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Zetva", b =>
                {
                    b.HasOne("MojAtar.Core.Domain.Radnja", null)
                        .WithOne()
                        .HasForeignKey("MojAtar.Core.Domain.Zetva", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MojAtar.Core.Domain.Kultura", "Kultura")
                        .WithMany()
                        .HasForeignKey("KulturaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Kultura");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.KatastarskaOpstina", b =>
                {
                    b.Navigation("Parcele");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Korisnik", b =>
                {
                    b.Navigation("Parcele");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Kultura", b =>
                {
                    b.Navigation("ParceleKulture");
                });

            modelBuilder.Entity("MojAtar.Core.Domain.Parcela", b =>
                {
                    b.Navigation("ParceleKulture");

                    b.Navigation("Radnje");
                });
#pragma warning restore 612, 618
        }
    }
}
