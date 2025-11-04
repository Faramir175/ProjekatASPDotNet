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

    }
}
