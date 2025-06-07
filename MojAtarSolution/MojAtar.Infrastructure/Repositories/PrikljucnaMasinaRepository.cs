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
    public class PrikljucnaMasinaRepository : IPrikljucnaMasinaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public PrikljucnaMasinaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public async Task<List<PrikljucnaMasina>> GetAll()
        {
            return await _dbContext.PrikljucneMasine.ToListAsync();
        }

        public async Task<List<PrikljucnaMasina>> GetAllByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.PrikljucneMasine
                .Where(p => p.IdKorisnik == idKorisnik)
                .ToListAsync();
        }


        public async Task<PrikljucnaMasina> GetById(Guid? id)
        {
            return await _dbContext.PrikljucneMasine.FindAsync(id);
        }

        public async Task<PrikljucnaMasina> GetByNaziv(string naziv)
        {
            return await _dbContext.PrikljucneMasine.FirstOrDefaultAsync(pk => pk.Naziv == naziv);

        }

        public async Task<PrikljucnaMasina> Add(PrikljucnaMasina entity)
        {
             _dbContext.PrikljucneMasine.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<PrikljucnaMasina> Update(PrikljucnaMasina entity)
        {
            PrikljucnaMasina? prikljucnaMasinaZaUpdate = await _dbContext.PrikljucneMasine.FirstOrDefaultAsync(temp => temp.Id == entity.Id);

            if (prikljucnaMasinaZaUpdate == null)
                return entity;

            prikljucnaMasinaZaUpdate.Id = entity.Id;
            prikljucnaMasinaZaUpdate.Naziv = entity.Naziv;
            prikljucnaMasinaZaUpdate.TipMasine = entity.TipMasine;
            prikljucnaMasinaZaUpdate.SirinaObrade = entity.SirinaObrade;
            prikljucnaMasinaZaUpdate.PoslednjiServis = entity.PoslednjiServis;
            prikljucnaMasinaZaUpdate.OpisServisa = entity.OpisServisa;
            prikljucnaMasinaZaUpdate.IdKorisnik = entity.IdKorisnik;

            await _dbContext.SaveChangesAsync();

            return prikljucnaMasinaZaUpdate;
        }

        public async Task<bool> Delete(PrikljucnaMasina entity)
        {
            _dbContext.PrikljucneMasine.RemoveRange(_dbContext.PrikljucneMasine.Where(temp => temp == entity));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<bool> DeletePrikljucnaMasinaById(Guid? id)
        {
            _dbContext.PrikljucneMasine.RemoveRange(_dbContext.PrikljucneMasine.Where(temp => temp.Id == id));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public Task<int> CountByKorisnikId(Guid korisnikId) =>
            _dbContext.PrikljucneMasine.CountAsync(pm => pm.IdKorisnik == korisnikId);

    }
}
