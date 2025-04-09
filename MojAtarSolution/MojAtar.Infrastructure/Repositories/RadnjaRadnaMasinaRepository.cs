using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Infrastructure.MojAtar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Infrastructure.Repositories
{
    public class RadnjaRadnaMasinaRepository : IRadnjaRadnaMasinaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public RadnjaRadnaMasinaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Radnja_RadnaMasina> Add(Radnja_RadnaMasina entity)
        {
            _dbContext.RadnjeRadneMasine.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Radnja_RadnaMasina> Update(Radnja_RadnaMasina entity)
        {
            var entitetIzBaze = await _dbContext.RadnjeRadneMasine
                .FirstOrDefaultAsync(x => x.IdRadnja == entity.IdRadnja && x.IdRadnaMasina == entity.IdRadnaMasina);

            if (entitetIzBaze == null) return entity;

            entitetIzBaze.BrojRadnihSati = entity.BrojRadnihSati;

            await _dbContext.SaveChangesAsync();
            return entitetIzBaze;
        }

        public async Task<bool> Delete(Radnja_RadnaMasina entity)
        {
            _dbContext.RadnjeRadneMasine.Remove(entity);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<Radnja_RadnaMasina> GetById(Guid idRadnja, Guid idRadnaMasina)
        {
            return await _dbContext.RadnjeRadneMasine
                .Include(r => r.RadnaMasina)
                .FirstOrDefaultAsync(x => x.IdRadnja == idRadnja && x.IdRadnaMasina == idRadnaMasina);
        }

        public async Task<List<Radnja_RadnaMasina>> GetAllByRadnjaId(Guid idRadnja)
        {
            return await _dbContext.RadnjeRadneMasine
                .Where(x => x.IdRadnja == idRadnja)
                .Include(x => x.RadnaMasina)
                .ToListAsync();
        }
        public async Task<List<Radnja_RadnaMasina>> GetAllByKorisnikId(Guid idKorisnik)
        {
            return await _dbContext.RadnjeRadneMasine
                .Include(x => x.RadnaMasina)
                .Where(x => x.RadnaMasina.IdKorisnik == idKorisnik)
                .ToListAsync();
        }

    }
}
