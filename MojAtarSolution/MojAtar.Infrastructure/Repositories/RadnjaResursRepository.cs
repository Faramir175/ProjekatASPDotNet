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
    public class RadnjaResursRepository : IRadnjaResursRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public RadnjaResursRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Radnja_Resurs> Add(Radnja_Resurs entity)
        {
            _dbContext.RadnjeResursi.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Radnja_Resurs> Update(Radnja_Resurs entity)
        {
            var entitetIzBaze = await _dbContext.RadnjeResursi
                .FirstOrDefaultAsync(x => x.IdRadnja == entity.IdRadnja && x.IdResurs == entity.IdResurs);

            if (entitetIzBaze == null) return entity;

            entitetIzBaze.Kolicina = entity.Kolicina;
            entitetIzBaze.DatumKoriscenja = entity.DatumKoriscenja;

            await _dbContext.SaveChangesAsync();
            return entitetIzBaze;
        }

        public async Task<bool> Delete(Radnja_Resurs entity)
        {
            _dbContext.RadnjeResursi.Remove(entity);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<Radnja_Resurs> GetById(Guid idRadnja, Guid idResurs)
        {
            return await _dbContext.RadnjeResursi
                .Include(r => r.Resurs)
                .FirstOrDefaultAsync(x => x.IdRadnja == idRadnja && x.IdResurs == idResurs);
        }

        public async Task<List<Radnja_Resurs>> GetAllByRadnjaId(Guid idRadnja)
        {
            return await _dbContext.RadnjeResursi
                .Where(x => x.IdRadnja == idRadnja)
                .Include(x => x.Resurs)
                .ToListAsync();
        }
        public async Task<List<Radnja_Resurs>> GetAllByKorisnikId(Guid idKorisnik)
        {
            return await _dbContext.RadnjeResursi
                .Include(x => x.Resurs)
                .Where(x => x.Resurs.IdKorisnik == idKorisnik)
                .ToListAsync();
        }

    }
}
