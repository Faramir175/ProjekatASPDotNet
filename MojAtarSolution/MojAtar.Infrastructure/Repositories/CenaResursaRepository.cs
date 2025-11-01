using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Infrastructure.MojAtar;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Infrastructure.Repositories
{
    public class CenaResursaRepository : ICenaResursaRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public CenaResursaRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CenaResursa>> GetPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.CeneResursa
                .Include(c => c.Resurs)
                .Where(c => c.Resurs.IdKorisnik == idKorisnik)
                .OrderByDescending(c => c.DatumVaznosti)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalCount(Guid idKorisnik)
        {
            return await _dbContext.CeneResursa
                .CountAsync(c => c.Resurs.IdKorisnik == idKorisnik);
        }

        public async Task<double> GetAktuelnaCena(Guid idKorisnik, Guid idResurs, DateTime datum)
        {
            return await _dbContext.CeneResursa
                .Where(c => c.IdResurs == idResurs && c.DatumVaznosti <= datum)
                .OrderByDescending(c => c.DatumVaznosti)
                .Select(c => c.CenaPojedinici)
                .FirstOrDefaultAsync();
        }

    }
}
