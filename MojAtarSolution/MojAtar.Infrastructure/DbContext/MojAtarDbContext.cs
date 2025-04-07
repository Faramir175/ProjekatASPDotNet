using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MojAtar.Core.Domain;

namespace MojAtar.Infrastructure.MojAtar
{
    public class MojAtarDbContext : DbContext
    {
        public MojAtarDbContext(DbContextOptions<MojAtarDbContext> options) : base(options) { }

        public DbSet<Parcela> Parcele { get; set; }
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<KatastarskaOpstina> KatastarskeOpstine { get; set; }
        public DbSet<Kultura> Kulture { get; set; }
        public DbSet<RadnaMasina> RadneMasine { get; set; }
        public DbSet<PrikljucnaMasina> PrikljucneMasine { get; set; }
        public DbSet<Radnja> Radnje { get; set; }
        public DbSet<Resurs> Resursi { get; set; }
        public DbSet<Parcela_Kultura> ParceleKulture { get; set; }
        public DbSet<CenaKulture> CeneKultura { get; set; }
        public DbSet<CenaResursa> CeneResursa { get; set; }
        public DbSet<Zetva> Zetve { get; set; }
        public DbSet<Radnja_PrikljucnaMasina> RadnjePrikljucneMasine { get; set; }
        public DbSet<Radnja_RadnaMasina> RadnjeRadneMasine { get; set; }
        public DbSet<Radnja_Resurs> RadnjeResursi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Podešavanje auto-incrementa za primarne ključeve
            modelBuilder.Entity<Korisnik>().Property(k => k.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Parcela>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<KatastarskaOpstina>().Property(k => k.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Kultura>().Property(k => k.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<RadnaMasina>().Property(rm => rm.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<PrikljucnaMasina>().Property(pm => pm.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Radnja>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Resurs>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Zetva>().Property(z => z.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<CenaKulture>().Property(ck => ck.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<CenaResursa>().Property(cr => cr.Id).ValueGeneratedOnAdd();

            // Podešavanje relacija M:N
            modelBuilder.Entity<Parcela_Kultura>().HasKey(pk => new { pk.IdParcela, pk.IdKultura });
            modelBuilder.Entity<Parcela_Kultura>()
                .HasOne(pk => pk.Parcela)
                .WithMany(p => p.ParceleKulture)
                .HasForeignKey(pk => pk.IdParcela);
            modelBuilder.Entity<Parcela_Kultura>()
                .HasOne(pk => pk.Kultura)
                .WithMany(k => k.ParceleKulture)
                .HasForeignKey(pk => pk.IdKultura);

            modelBuilder.Entity<Radnja_PrikljucnaMasina>().HasKey(rpm => new { rpm.IdRadnja, rpm.IdPrikljucnaMasina });
            modelBuilder.Entity<Radnja_PrikljucnaMasina>()
                .HasOne(rpm => rpm.Radnja)
                .WithMany()
                .HasForeignKey(rpm => rpm.IdRadnja);
            modelBuilder.Entity<Radnja_PrikljucnaMasina>()
                .HasOne(rpm => rpm.PrikljucnaMasina)
                .WithMany()
                .HasForeignKey(rpm => rpm.IdPrikljucnaMasina);

            modelBuilder.Entity<Radnja_RadnaMasina>().HasKey(rrm => new { rrm.IdRadnja, rrm.IdRadnaMasina });
            modelBuilder.Entity<Radnja_RadnaMasina>()
                .HasOne(rrm => rrm.Radnja)
                .WithMany()
                .HasForeignKey(rrm => rrm.IdRadnja);
            modelBuilder.Entity<Radnja_RadnaMasina>()
                .HasOne(rrm => rrm.RadnaMasina)
                .WithMany()
                .HasForeignKey(rrm => rrm.IdRadnaMasina);

            modelBuilder.Entity<Radnja_Resurs>().HasKey(rr => new { rr.IdRadnja, rr.IdResurs });
            modelBuilder.Entity<Radnja_Resurs>()
                .HasOne(rr => rr.Radnja)
                .WithMany()
                .HasForeignKey(rr => rr.IdRadnja);
            modelBuilder.Entity<Radnja_Resurs>()
                .HasOne(rr => rr.Resurs)
                .WithMany()
                .HasForeignKey(rr => rr.IdResurs);

            modelBuilder.Entity<Parcela>()
                .HasOne(p => p.KatastarskaOpstina)
                .WithMany(k => k.Parcele)
                .HasForeignKey(p => p.IdKatastarskaOpstina);

            modelBuilder.Entity<Parcela>()
                .HasOne(p => p.Korisnik)
                .WithMany(k => k.Parcele)
                .HasForeignKey(p => p.IdKorisnik);

            modelBuilder.Entity<CenaKulture>()
                .HasOne(ck => ck.Kultura)
                .WithMany(k => k.CeneKulture)
                .HasForeignKey(ck => ck.IdKultura);

            modelBuilder.Entity<CenaResursa>()
                .HasOne(cr => cr.Resurs)
                .WithMany(k => k.CeneResursa)
                .HasForeignKey(cr => cr.IdResurs);

            // Relacija Radnja - Parcela
            modelBuilder.Entity<Radnja>()
                .HasOne(r => r.Parcela)
                .WithMany(p => p.Radnje)
                .HasForeignKey(r => r.IdParcela);

            // Relacija Radnja - Kultura (pošto više nije u Zetvi)
            modelBuilder.Entity<Radnja>()
                .HasOne(r => r.Kultura)
                .WithMany(k => k.Radnje)
                .HasForeignKey(r => r.IdKultura)
                .IsRequired(false); // jer nije obavezna za sve radnje

            // Nasleđivanje Radnja - Zetva
            modelBuilder.Entity<Radnja>()
                .HasDiscriminator<string>("RadnjaTip")
                .HasValue<Radnja>("Radnja")
                .HasValue<Zetva>("Zetva");


        }

    }
}
