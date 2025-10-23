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
    public class KulturaRepository : IKulturaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public KulturaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public async Task<List<Kultura>> GetAll()
        {
            return await _dbContext.Kulture.ToListAsync();
        }

        public async Task<List<Kultura>> GetAllByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.Kulture
                .Where(p => p.IdKorisnik == idKorisnik)
                .ToListAsync();
        }
        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.Kulture.CountAsync(x => x.IdKorisnik == idKorisnik);
        }

        public async Task<List<Kultura>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.Kulture
                .Where(x => x.IdKorisnik == idKorisnik)
                .OrderBy(x => x.Naziv)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }


        public async Task<Kultura> GetById(Guid? id)
        {
            return await _dbContext.Kulture.FindAsync(id);
        }

        public async Task<Kultura> GetByNaziv(string naziv)
        {
            return await _dbContext.Kulture.FirstOrDefaultAsync(k => k.Naziv == naziv);

        }

        public async Task<Kultura> Add(Kultura entity)
        {
             _dbContext.Kulture.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Kultura> Update(Kultura entity)
        {
            Kultura? kulturaZaUpdate = await _dbContext.Kulture.FirstOrDefaultAsync(temp => temp.Id == entity.Id);

            if (kulturaZaUpdate == null)
                return entity;

            kulturaZaUpdate.Id = entity.Id;
            kulturaZaUpdate.Naziv = entity.Naziv;
            kulturaZaUpdate.Hibrid = entity.Hibrid;
            kulturaZaUpdate.AktuelnaCena = entity.AktuelnaCena;
            kulturaZaUpdate.IdKorisnik = entity.IdKorisnik;

            await _dbContext.SaveChangesAsync();

            return kulturaZaUpdate;
        }

        public async Task<bool> Delete(Kultura entity)
        {
            _dbContext.Kulture.RemoveRange(_dbContext.Kulture.Where(temp => temp == entity));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteKulturaById(Guid? id)
        {
            _dbContext.Kulture.RemoveRange(_dbContext.Kulture.Where(temp => temp.Id == id));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task DodajCenu(CenaKulture cena)
        {
            _dbContext.CeneKultura.AddAsync(cena);
            await _dbContext.SaveChangesAsync();
        }
        public Task<int> CountByKorisnikId(Guid korisnikId) =>
            _dbContext.Kulture.CountAsync(k => k.IdKorisnik == korisnikId);

    }
}
