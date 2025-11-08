using Microsoft.EntityFrameworkCore;
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
        public DbSet<Prodaja> Prodaje { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =====================================
            // AUTO-INCREMENT za primarne ključeve
            // =====================================
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

            // =====================================
            // M:N RELACIJE
            // =====================================

            //  Parcela_Kultura — briše se ako se obriše parcela ili kultura
            modelBuilder.Entity<Parcela_Kultura>()
                .HasKey(pk => pk.Id);
            modelBuilder.Entity<Parcela_Kultura>()
                .HasOne(pk => pk.Parcela)
                .WithMany(p => p.ParceleKulture)
                .HasForeignKey(pk => pk.IdParcela)
                .OnDelete(DeleteBehavior.Cascade); //  Briše ako se obriše parcela
            modelBuilder.Entity<Parcela_Kultura>()
                .HasOne(pk => pk.Kultura)
                .WithMany(k => k.ParceleKulture)
                .HasForeignKey(pk => pk.IdKultura)
                .OnDelete(DeleteBehavior.Cascade); //  Briše ako se obriše kultura

            //  Radnja_PrikljucnaMasina — veza nestaje ako se obriše radnja ili priključna mašina
            modelBuilder.Entity<Radnja_PrikljucnaMasina>()
                .HasKey(rpm => new { rpm.IdRadnja, rpm.IdPrikljucnaMasina });
            modelBuilder.Entity<Radnja_PrikljucnaMasina>()
                .HasOne(rpm => rpm.Radnja)
                .WithMany(r => r.RadnjePrikljucneMasine)
                .HasForeignKey(rpm => rpm.IdRadnja)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Radnja_PrikljucnaMasina>()
                .HasOne(rpm => rpm.PrikljucnaMasina)
                .WithMany()
                .HasForeignKey(rpm => rpm.IdPrikljucnaMasina)
                .OnDelete(DeleteBehavior.Cascade);

            //  Radnja_RadnaMasina — veza nestaje ako se obriše radnja ili radna mašina
            modelBuilder.Entity<Radnja_RadnaMasina>()
                .HasKey(rrm => new { rrm.IdRadnja, rrm.IdRadnaMasina });
            modelBuilder.Entity<Radnja_RadnaMasina>()
                .HasOne(rrm => rrm.Radnja)
                .WithMany(r => r.RadnjeRadneMasine)
                .HasForeignKey(rrm => rrm.IdRadnja)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Radnja_RadnaMasina>()
                .HasOne(rrm => rrm.RadnaMasina)
                .WithMany()
                .HasForeignKey(rrm => rrm.IdRadnaMasina)
                .OnDelete(DeleteBehavior.Cascade);

            //  Radnja_Resurs — briše se ako se obriše radnja ili resurs
            modelBuilder.Entity<Radnja_Resurs>()
                .HasKey(rr => new { rr.IdRadnja, rr.IdResurs });
            modelBuilder.Entity<Radnja_Resurs>()
                .HasOne(rr => rr.Radnja)
                .WithMany(r => r.RadnjeResursi)
                .HasForeignKey(rr => rr.IdRadnja)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Radnja_Resurs>()
                .HasOne(rr => rr.Resurs)
                .WithMany()
                .HasForeignKey(rr => rr.IdResurs)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================
            // POJEDINAČNE RELACIJE
            // =====================================

            modelBuilder.Entity<Parcela>()
                .HasOne(p => p.KatastarskaOpstina)
                .WithMany(k => k.Parcele)
                .HasForeignKey(p => p.IdKatastarskaOpstina)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Parcela>()
                .HasOne(p => p.Korisnik)
                .WithMany(k => k.Parcele)
                .HasForeignKey(p => p.IdKorisnik)
                .OnDelete(DeleteBehavior.Cascade);

            //  Cene Kulture i Resursa — brišu se ako se obriše kultura/resurs
            modelBuilder.Entity<CenaKulture>()
                .HasOne(ck => ck.Kultura)
                .WithMany(k => k.CeneKulture)
                .HasForeignKey(ck => ck.IdKultura)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CenaResursa>()
                .HasOne(cr => cr.Resurs)
                .WithMany(r => r.CeneResursa)
                .HasForeignKey(cr => cr.IdResurs)
                .OnDelete(DeleteBehavior.Cascade);

            //  Radnja - Parcela: ako se obriše parcela, brišu se i radnje
            modelBuilder.Entity<Radnja>()
                .HasOne(r => r.Parcela)
                .WithMany(p => p.Radnje)
                .HasForeignKey(r => r.IdParcela)
                .OnDelete(DeleteBehavior.Cascade);

            //  Radnja - Kultura: ako se obriše kultura, postaje null
            modelBuilder.Entity<Radnja>()
                .HasOne(r => r.Kultura)
                .WithMany(k => k.Radnje)
                .HasForeignKey(r => r.IdKultura)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            //  Prodaja - Kultura: ako se obriše kultura, brišu se i prodaje
            modelBuilder.Entity<Prodaja>()
                .HasOne(p => p.Kultura)
                .WithMany(k => k.Prodaje)
                .HasForeignKey(p => p.IdKultura)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================
            // NASLEĐIVANJE RADNJA - ŽETVA
            // =====================================
            modelBuilder.Entity<Radnja>()
                .HasDiscriminator<string>("RadnjaTip")
                .HasValue<Radnja>("Radnja")
                .HasValue<Zetva>("Zetva");

            // =====================================
            // PRECIZNOST POVRŠINA
            // =====================================
            modelBuilder.Entity<Parcela>()
                .Property(p => p.Povrsina)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<Parcela_Kultura>()
                .Property(pk => pk.Povrsina)
                .HasColumnType("decimal(18,4)");

        }


    }
}
