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
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly MojAtarDbContext _dbContext;
        public KorisnikRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public async Task<List<Korisnik>> GetAll()
        {
            return await _dbContext.Korisnici.ToListAsync();
        }

        public async Task<Korisnik> GetById(Guid id)
        {
            return await _dbContext.Korisnici.FindAsync(id);
        }

        public async Task<Korisnik> GetByTipKorisnika(KorisnikTip tipKorisnika)
        {
            return await _dbContext.Korisnici.FirstOrDefaultAsync(k => k.TipKorisnika == tipKorisnika);
        }
        public async Task<Korisnik> GetByEmail(string email)
        {
            return await _dbContext.Korisnici.FirstOrDefaultAsync(k => k.Email == email);

        }

        public async Task<Korisnik> Add(Korisnik entity)
        {
             _dbContext.Korisnici.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        Task<Korisnik> IRepository<Korisnik>.Update(Korisnik entity)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<Korisnik>.Delete(Korisnik entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteKorisnikById(Guid? id)
        {
            _dbContext.Korisnici.RemoveRange(_dbContext.Korisnici.Where(temp => temp.Id == id));
            int rowsDeleted = await _dbContext.SaveChangesAsync();

            return rowsDeleted > 0;
        }
    }
}
