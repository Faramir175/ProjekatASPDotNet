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
    public class ParcelaRepository : IParcelaRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public ParcelaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public async Task<List<Parcela>> GetAll()
        {
            return await _dbContext.Parcele.ToListAsync();
        }

        public async Task<List<Parcela>> GetAllByKorisnik(Guid idKorisnik)
        {
            return await _dbContext.Parcele
                .Where(p => p.IdKorisnik == idKorisnik)
                .ToListAsync();
        }


        public async Task<Parcela> GetById(Guid? id)
        {
            return await _dbContext.Parcele.FindAsync(id);
        }

        public async Task<Parcela> GetByNaziv(string naziv)
        {
            return await _dbContext.Parcele.FirstOrDefaultAsync(k => k.Naziv == naziv);

        }

        public async Task<Parcela> Add(Parcela entity)
        {
             _dbContext.Parcele.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Parcela> Update(Parcela entity)
        {
            Parcela? parcelaZaUpdate = await _dbContext.Parcele.FirstOrDefaultAsync(temp => temp.Id == entity.Id);

            if (parcelaZaUpdate == null)
                return entity;

            parcelaZaUpdate.Id = entity.Id;
            parcelaZaUpdate.BrojParcele = entity.BrojParcele;
            parcelaZaUpdate.Naziv = entity.Naziv;
            parcelaZaUpdate.Povrsina = entity.Povrsina;
            parcelaZaUpdate.Napomena = entity.Napomena;
            parcelaZaUpdate.IdKatastarskaOpstina = entity.IdKatastarskaOpstina;
            parcelaZaUpdate.IdKorisnik = entity.IdKorisnik;

            await _dbContext.SaveChangesAsync();

            return parcelaZaUpdate;
        }

        public async Task<bool> Delete(Parcela entity)
        {
            _dbContext.Parcele.RemoveRange(_dbContext.Parcele.Where(temp => temp == entity));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<bool> DeleteParcelaById(Guid? id)
        {
            _dbContext.Parcele.RemoveRange(_dbContext.Parcele.Where(temp => temp.Id == id));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public Task<int> CountByKorisnikId(Guid korisnikId)
        {
            return _dbContext.Parcele.CountAsync(p => p.IdKorisnik == korisnikId);
        }
    }
}
