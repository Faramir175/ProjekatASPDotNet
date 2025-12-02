using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Infrastructure.MojAtar;

namespace MojAtar.Infrastructure.Repositories
{
    public class RadnjaRepository : IRadnjaRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public RadnjaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Radnja>> GetAll()
        {
            return await _dbContext.Radnje
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .ToListAsync();
        }

        public async Task<Radnja> GetById(Guid? id)
        {
            // EF Core automatski prepoznaje da li je Radnja ili Zetva na osnovu Discriminator kolone
            // Nema potrebe za ručnim kastovanjem 'as Zetva'
            return await _dbContext.Radnje
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Radnja> GetByTipRadnje(RadnjaTip tipRadnje)
        {
            return await _dbContext.Radnje
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .FirstOrDefaultAsync(r => r.TipRadnje == tipRadnje);
        }

        public async Task<Radnja> Add(Radnja entity)
        {
            _dbContext.Radnje.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Radnja> Update(Radnja entity)
        {
            // 🛑 BITNA PROMENA:
            // Pošto servis koristi "Dohvati -> Izmeni -> Sačuvaj" pristup,
            // 'entity' koji stigne ovde je već "Tracked" (praćen) od strane DbContext-a.
            // Nema potrebe da ga ponovo tražimo u bazi ili ručno mapiramo polja.
            // EF Core već zna šta je promenjeno u servisu.

            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Radnja entity)
        {
            _dbContext.Radnje.Remove(entity);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        // Ovu metodu smo zadržali ako je negde specifično koristiš, 
        // ali servis sada uglavnom radi GetById pa Delete(entity).
        public async Task<bool> DeleteRadnjaById(Guid? id)
        {
            var radnja = await _dbContext.Radnje.FindAsync(id);
            if (radnja == null) return false;

            _dbContext.Radnje.Remove(radnja);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<List<Radnja>> GetAllByParcela(Guid idParcela)
        {
            return await _dbContext.Radnje
                .Where(r => r.IdParcela == idParcela)
                .Include(r => r.Kultura)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetAllByKultura(Guid idKultura)
        {
            return await _dbContext.Radnje
                .Where(r => r.IdKultura == idKultura)
                .Include(r => r.Parcela)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .ToListAsync();
        }

        public async Task<decimal> GetUkupanPrinosZaParcelu(Guid idParcela)
        {
            var ukupno = await _dbContext.Radnje
                .OfType<Zetva>() // Filtrira samo Zetve
                .Where(z => z.IdParcela == idParcela)
                .SumAsync(z => z.Prinos);

            return (decimal)ukupno;
        }

        public async Task<int> GetCountByParcela(Guid idParcela)
        {
            return await _dbContext.Radnje
                .Where(x => x.IdParcela == idParcela)
                .CountAsync();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.Radnje
                .Where(x => x.Parcela.IdKorisnik == idKorisnik)
                .CountAsync();
        }

        public async Task<List<Radnja>> GetAllByParcelaPaged(Guid idParcela, int skip, int take)
        {
            return await _dbContext.Radnje
                .Where(x => x.IdParcela == idParcela)
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(x => x.DatumIzvrsenja)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.Radnje
                .Where(x => x.Parcela.IdKorisnik == idKorisnik)
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(x => x.DatumIzvrsenja)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetLastRadnjeByKorisnik(Guid korisnikId, int broj)
        {
            return await _dbContext.Radnje
                .Where(r => r.Parcela.IdKorisnik == korisnikId)
                .Include(r => r.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .Take(broj)
                .ToListAsync();
        }

        public Task<int> CountByKorisnikId(Guid korisnikId)
        {
            return _dbContext.Radnje.CountAsync(p => p.Parcela.IdKorisnik == korisnikId);
        }

        public async Task<Parcela> GetParcelaSaSetvama(Guid idParcela)
        {
            // AsNoTracking je ovde OK jer samo čitamo radi provere
            return await _dbContext.Parcele
                .Include(p => p.ParceleKulture.Where(pk => pk.DatumZetve == null))
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == idParcela);
        }

        public async Task UpdateUkupanTrosak(Guid idRadnja)
        {
            // Dohvatamo radnju i njene resurse da preračunamo trošak
            // NAPOMENA: Ovde se koristi 'AktuelnaCena' resursa. 
            // Ako želiš istorijsku cenu, logika bi morala biti drugačija (u servisu), 
            // ali za sada ostavljamo ovako da ne menjamo ponašanje aplikacije.

            var radnja = await _dbContext.Radnje
                .Include(r => r.RadnjeResursi)
                    .ThenInclude(rr => rr.Resurs)
                .FirstOrDefaultAsync(r => r.Id == idRadnja);

            if (radnja == null) return;

            double noviTrosak = 0;
            if (radnja.RadnjeResursi != null)
            {
                foreach (var rr in radnja.RadnjeResursi)
                {
                    if (rr.Resurs != null)
                        noviTrosak += (double)(rr.Kolicina * rr.Resurs.AktuelnaCena);
                }
            }

            radnja.UkupanTrosak = noviTrosak;

            // Samo SaveChanges, jer je radnja već učitana (Tracked)
            await _dbContext.SaveChangesAsync();
        }
    }
}