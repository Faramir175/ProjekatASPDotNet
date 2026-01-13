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
                // VIŠE NEMA: .Include(r => r.Parcela)
                .Include(r => r.RadnjeParcele) // Učitavamo veznu tabelu
                    .ThenInclude(rp => rp.Parcela) // Učitavamo podatke o parceli (naziv itd.)
                .Include(r => r.Kultura)
                .ToListAsync();
        }

        public async Task<Radnja> GetById(Guid? id)
        {
            return await _dbContext.Radnje
                .Include(r => r.RadnjeParcele)
                    .ThenInclude(rp => rp.Parcela)
                .Include(r => r.Kultura)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Radnja> GetByTipRadnje(RadnjaTip tipRadnje)
        {
            return await _dbContext.Radnje
                .Include(r => r.RadnjeParcele)
                    .ThenInclude(rp => rp.Parcela)
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
            // Entity Framework već prati promene (Tracked entity), samo snimamo.
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(Radnja entity)
        {
            _dbContext.Radnje.Remove(entity);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteRadnjaById(Guid? id)
        {
            var radnja = await _dbContext.Radnje.FindAsync(id);
            if (radnja == null) return false;

            _dbContext.Radnje.Remove(radnja);
            int rowsDeleted = await _dbContext.SaveChangesAsync();
            return rowsDeleted > 0;
        }

        // --- METODE KOJE SU ZAHTEVALE NAJVEĆE IZMENE ---

        public async Task<List<Radnja>> GetAllByParcela(Guid idParcela)
        {
            return await _dbContext.Radnje
                // PROMENA: Tražimo radnje koje u svojoj listi parcela imaju traženi ID
                .Where(r => r.RadnjeParcele.Any(rp => rp.IdParcela == idParcela))
                .Include(r => r.Kultura)
                // Ovde ne moramo include-ovati sve parcele ako nam ne trebaju za prikaz, 
                // ali ako treba naziv, onda mora:
                .Include(r => r.RadnjeParcele).ThenInclude(rp => rp.Parcela)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetAllByKultura(Guid idKultura)
        {
            return await _dbContext.Radnje
                .Where(r => r.IdKultura == idKultura)
                .Include(r => r.RadnjeParcele)
                    .ThenInclude(rp => rp.Parcela)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .ToListAsync();
        }

        public async Task<decimal> GetUkupanPrinosZaParcelu(Guid idParcela)
        {
            // Oprez: Prinos se čuva na nivou RADNJE (Zetve), ne na nivou parcele u tabeli Zetva.
            // Ako je Zetva bila za 3 parcele, a Prinos je upisan 10 tona, to je ukupno 10 tona.
            // Ovde sabiramo sve zetve u kojima je ucestvovala ova parcela.
            // *Napomena:* Ovo nije idealno ako želiš precizan prinos SAMO sa te parcele, 
            // ali pošto u Zetva tabeli nemaš podatak po parceli (nego u RadnjaParcela), 
            // morali bi sabirati RadnjaParcela.Prinos ako bismo ga imali, ali RadnjaParcela ima samo Povrsinu.

            // AKO si hteo samo zetve koje uključuju ovu parcelu:
            var ukupno = await _dbContext.Radnje
                .OfType<Zetva>()
                .Where(z => z.RadnjeParcele.Any(rp => rp.IdParcela == idParcela))
                .SumAsync(z => z.Prinos);

            // *Napomena za ubuduće:* Ako ti treba tačan prinos po parceli, trebalo bi da ga čuvaš u RadnjaParcela tabeli, 
            // a ne samo ukupno u Zetva tabeli. Za sada vraćamo zbir kao i pre.
            return (decimal)ukupno;
        }

        public async Task<int> GetCountByParcela(Guid idParcela)
        {
            return await _dbContext.Radnje
                .Where(x => x.RadnjeParcele.Any(rp => rp.IdParcela == idParcela))
                .CountAsync();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            // Tražimo radnje koje imaju BAR JEDNU parcelu koja pripada tom korisniku
            return await _dbContext.Radnje
                .Where(x => x.RadnjeParcele.Any(rp => rp.Parcela.IdKorisnik == idKorisnik))
                .CountAsync();
        }

        public async Task<List<Radnja>> GetAllByParcelaPaged(Guid idParcela, int skip, int take)
        {
            return await _dbContext.Radnje
                // Filtriranje: Daj mi radnje gde se u listi parcela nalazi ova parcela
                .Where(x => x.RadnjeParcele.Any(rp => rp.IdParcela == idParcela))
                .Include(r => r.RadnjeParcele)
                    .ThenInclude(rp => rp.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(x => x.DatumIzvrsenja)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.Radnje
                // Filtriranje: Daj mi radnje korisnika (bilo koja parcela u radnji da je njegova)
                .Where(x => x.RadnjeParcele.Any(rp => rp.Parcela.IdKorisnik == idKorisnik))
                .Include(r => r.RadnjeParcele)
                    .ThenInclude(rp => rp.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(x => x.DatumIzvrsenja)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Radnja>> GetLastRadnjeByKorisnik(Guid korisnikId, int broj)
        {
            return await _dbContext.Radnje
                .Where(r => r.RadnjeParcele.Any(rp => rp.Parcela.IdKorisnik == korisnikId))
                .Include(r => r.RadnjeParcele)
                    .ThenInclude(rp => rp.Parcela)
                .Include(r => r.Kultura)
                .OrderByDescending(r => r.DatumIzvrsenja)
                .Take(broj)
                .ToListAsync();
        }

        public Task<int> CountByKorisnikId(Guid korisnikId)
        {
            return _dbContext.Radnje
                .Where(x => x.RadnjeParcele.Any(rp => rp.Parcela.IdKorisnik == korisnikId))
                .CountAsync();
        }

        public async Task<Parcela> GetParcelaSaSetvama(Guid idParcela)
        {
            // Ovo ostaje isto jer gleda ParcelaKultura tabelu koja je vezana za Parcelu direktno
            return await _dbContext.Parcele
                .Include(p => p.ParceleKulture.Where(pk => pk.DatumZetve == null))
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == idParcela);
        }

        public async Task UpdateUkupanTrosak(Guid idRadnja)
        {
            // Ovde moramo paziti na Includes
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
            await _dbContext.SaveChangesAsync();
        }
    }
}