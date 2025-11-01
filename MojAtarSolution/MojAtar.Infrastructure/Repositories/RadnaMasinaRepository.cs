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
    public class RadnaMasinaRepository : IRadnaMasinaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public RadnaMasinaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public async Task<List<RadnaMasina>> GetAll()
        {
            return await _dbContext.RadneMasine.ToListAsync();
        }

        public async Task<List<RadnaMasina>> GetAllByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.RadneMasine
                .Where(p => p.IdKorisnik == idKorisnik)
                .ToListAsync();
        }


        public async Task<RadnaMasina> GetById(Guid? id)
        {
            return await _dbContext.RadneMasine.FindAsync(id);
        }

        public async Task<RadnaMasina> GetByNazivIKorisnik(string naziv, Guid idKorisnik)
        {
            return await _dbContext.RadneMasine.FirstOrDefaultAsync(k => k.Naziv == naziv && k.IdKorisnik == idKorisnik);

        }

        public async Task<RadnaMasina> Add(RadnaMasina entity)
        {
             _dbContext.RadneMasine.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<RadnaMasina> Update(RadnaMasina entity)
        {
            RadnaMasina? radnaMasinaZaUpdate = await _dbContext.RadneMasine.FirstOrDefaultAsync(temp => temp.Id == entity.Id);

            if (radnaMasinaZaUpdate == null)
                return entity;

            radnaMasinaZaUpdate.Id = entity.Id;
            radnaMasinaZaUpdate.Naziv = entity.Naziv;
            radnaMasinaZaUpdate.TipUlja = entity.TipUlja;
            radnaMasinaZaUpdate.RadniSatiServis = entity.RadniSatiServis;
            radnaMasinaZaUpdate.PoslednjiServis = entity.PoslednjiServis;
            radnaMasinaZaUpdate.OpisServisa = entity.OpisServisa;
            radnaMasinaZaUpdate.UkupanBrojRadnihSati = entity.UkupanBrojRadnihSati;
            radnaMasinaZaUpdate.IdKorisnik = entity.IdKorisnik;

            await _dbContext.SaveChangesAsync();

            return radnaMasinaZaUpdate;
        }

        public async Task<bool> Delete(RadnaMasina entity)
        {
            _dbContext.RadneMasine.RemoveRange(_dbContext.RadneMasine.Where(temp => temp == entity));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteRadnaMasinaById(Guid? id)
        {
            _dbContext.RadneMasine.RemoveRange(_dbContext.RadneMasine.Where(temp => temp.Id == id));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }
        public Task<int> CountByKorisnikId(Guid korisnikId) =>
            _dbContext.RadneMasine.CountAsync(rm => rm.IdKorisnik == korisnikId);

        public async Task<List<RadnaMasina>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.RadneMasine
                .Where(rm => rm.IdKorisnik == idKorisnik)
                .OrderBy(rm => rm.Naziv)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.RadneMasine.CountAsync(rm => rm.IdKorisnik == idKorisnik);
        }
    }
}
