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
    public class RadnjaPrikljucnaMasinaRepository : IRadnjaPrikljucnaMasinaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public RadnjaPrikljucnaMasinaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Radnja_PrikljucnaMasina> Add(Radnja_PrikljucnaMasina entity)
        {
            _dbContext.RadnjePrikljucneMasine.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Radnja_PrikljucnaMasina> Update(Radnja_PrikljucnaMasina entity)
        {
            var entitetIzBaze = await _dbContext.RadnjePrikljucneMasine
                .FirstOrDefaultAsync(x => x.IdRadnja == entity.IdRadnja && x.IdPrikljucnaMasina == entity.IdPrikljucnaMasina);

            if (entitetIzBaze == null) return entity;

            entitetIzBaze.BrojRadnihSati = entity.BrojRadnihSati;

            await _dbContext.SaveChangesAsync();
            return entitetIzBaze;
        }

        public async Task<bool> Delete(Radnja_PrikljucnaMasina entity)
        {
            _dbContext.RadnjePrikljucneMasine.Remove(entity);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<Radnja_PrikljucnaMasina> GetById(Guid idRadnja, Guid idPrikljucnaMasina)
        {
            return await _dbContext.RadnjePrikljucneMasine
                .Include(r => r.PrikljucnaMasina)
                .FirstOrDefaultAsync(x => x.IdRadnja == idRadnja && x.IdPrikljucnaMasina == idPrikljucnaMasina);
        }

        public async Task<List<Radnja_PrikljucnaMasina>> GetAllByRadnjaId(Guid idRadnja)
        {
            return await _dbContext.RadnjePrikljucneMasine
                .Where(x => x.IdRadnja == idRadnja)
                .Include(x => x.PrikljucnaMasina)
                .ToListAsync();
        }
        public async Task<List<Radnja_PrikljucnaMasina>> GetAllByKorisnikId(Guid idKorisnik)
        {
            return await _dbContext.RadnjePrikljucneMasine
                .Include(x => x.PrikljucnaMasina)
                .Where(x => x.PrikljucnaMasina.IdKorisnik == idKorisnik)
                .ToListAsync();
        }

    }
}
