using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Infrastructure.MojAtar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Infrastructure.Repositories
{
    public class ProdajaRepository : IProdajaRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public ProdajaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Prodaja>> GetAllByKorisnik(Guid korisnikId)
        {
            return await _dbContext.Prodaje
                .Include(p => p.Kultura)
                .Where(p => p.Kultura.IdKorisnik == korisnikId)
                .OrderByDescending(p => p.DatumProdaje)
                .ToListAsync();
        }

        public async Task<Prodaja?> GetById(Guid id)
        {
            return await _dbContext.Prodaje
                .Include(p => p.Kultura)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Prodaja> Add(Prodaja prodaja)
        {
            await _dbContext.Prodaje.AddAsync(prodaja);
            await _dbContext.SaveChangesAsync();
            return prodaja;
        }

        public async Task Delete(Guid id)
        {
            var entity = await _dbContext.Prodaje.FindAsync(id);
            if (entity != null)
            {
                _dbContext.Prodaje.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<Prodaja> Update(Prodaja prodaja)
        {
            var existing = await _dbContext.Prodaje.FirstOrDefaultAsync(p => p.Id == prodaja.Id);
            if (existing == null)
                throw new KeyNotFoundException("Prodaja nije pronađena.");

            existing.IdKultura = prodaja.IdKultura;
            existing.Kolicina = prodaja.Kolicina;
            existing.CenaPoJedinici = prodaja.CenaPoJedinici;
            existing.DatumProdaje = prodaja.DatumProdaje;
            existing.Napomena = prodaja.Napomena;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<Kultura?> GetKulturaById(Guid idKultura)
        {
            return await _dbContext.Kulture.FirstOrDefaultAsync(k => k.Id == idKultura);
        }

        public async Task<List<Prodaja>> GetPaged(Guid korisnikId, int skip, int take)
        {
            return await _dbContext.Prodaje
                .Include(p => p.Kultura)
                .Where(p => p.Kultura.IdKorisnik == korisnikId)
                .OrderByDescending(p => p.DatumProdaje)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalCount(Guid korisnikId)
        {
            return await _dbContext.Prodaje
                .Include(p => p.Kultura)
                .CountAsync(p => p.Kultura.IdKorisnik == korisnikId);
        }

        public async Task<decimal> GetUkupanPrinosZaKulturu(Guid idKultura)
        {
            return await _dbContext.Zetve
                .Where(z => z.IdKultura == idKultura)
                .SumAsync(z => (decimal)z.Prinos);
        }

        public async Task<decimal> GetUkupnoProdatoZaKulturu(Guid idKultura)
        {
            return await _dbContext.Prodaje
                .Where(p => p.IdKultura == idKultura)
                .SumAsync(p => p.Kolicina);
        }
        public async Task<List<Prodaja>> GetByKorisnikAndPeriod(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma)
        {
            DateTime? kraj = doDatuma?.AddDays(1).AddSeconds(-1);

            return await _dbContext.Prodaje
                .Include(p => p.Kultura)
                .Where(p => p.Kultura.IdKorisnik == korisnikId &&
                            (!odDatuma.HasValue || p.DatumProdaje >= odDatuma) &&
                            (!doDatuma.HasValue || p.DatumProdaje <= kraj))
                .OrderByDescending(p => p.DatumProdaje)
                .ToListAsync();
        }
        public async Task<Dictionary<Guid, decimal>> GetPrinosPoKulturi(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma)
        {
            var query = _dbContext.Zetve
                .Include(z => z.Kultura)
                .Where(z => z.Kultura.IdKorisnik == korisnikId);

            if (odDatuma.HasValue)
                query = query.Where(z => z.DatumIzvrsenja >= odDatuma);
            if (doDatuma.HasValue)
                query = query.Where(z => z.DatumIzvrsenja <= doDatuma);

            var rezultat = await query
                .Where(z => z.IdKultura != null)
                .GroupBy(z => z.IdKultura.Value)
                .Select(g => new
                {
                    IdKultura = g.Key,
                    Prinos = g.Sum(z => (decimal)z.Prinos)
                })
                .ToListAsync();

            return rezultat.ToDictionary(x => x.IdKultura, x => x.Prinos);

        }

    }
}
