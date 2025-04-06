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
    public class ResursRepository : IResursRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public ResursRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public async Task<List<Resurs>> GetAll()
        {
            return await _dbContext.Resursi.ToListAsync();
        }

        public async Task<List<Resurs>> GetAllByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.Resursi
                .Where(p => p.IdKorisnik == idKorisnik)
                .ToListAsync();
        }


        public async Task<Resurs> GetById(Guid? id)
        {
            return await _dbContext.Resursi.FindAsync(id);
        }

        public async Task<Resurs> GetByNaziv(string naziv)
        {
            return await _dbContext.Resursi.FirstOrDefaultAsync(k => k.Naziv == naziv);

        }

        public async Task<Resurs> Add(Resurs entity)
        {
             _dbContext.Resursi.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Resurs> Update(Resurs entity)
        {
            Resurs? resursZaUpdate = await _dbContext.Resursi.FirstOrDefaultAsync(temp => temp.Id == entity.Id);

            if (resursZaUpdate == null)
                return entity;

            resursZaUpdate.Id = entity.Id;
            resursZaUpdate.Naziv = entity.Naziv;
            resursZaUpdate.Vrsta = entity.Vrsta;
            resursZaUpdate.AktuelnaCena = entity.AktuelnaCena;
            resursZaUpdate.IdKorisnik = entity.IdKorisnik;

            await _dbContext.SaveChangesAsync();

            return resursZaUpdate;
        }

        public async Task<bool> Delete(Resurs entity)
        {
            _dbContext.Resursi.RemoveRange(_dbContext.Resursi.Where(temp => temp == entity));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteResursById(Guid? id)
        {
            _dbContext.Resursi.RemoveRange(_dbContext.Resursi.Where(temp => temp.Id == id));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }
    }
}
