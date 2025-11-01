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
    public class CenaKultureRepository : ICenaKultureRepository
    {
        private readonly MojAtarDbContext _dbContext;

        public CenaKultureRepository(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CenaKulture>> GetPaged(Guid idKorisnik, int skip, int take)
        {
            return await _dbContext.CeneKultura
                .Include(c => c.Kultura)
                .Where(c => c.Kultura.IdKorisnik == idKorisnik)
                .OrderByDescending(c => c.DatumVaznosti)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalCount(Guid idKorisnik)
        {
            return await _dbContext.CeneKultura
                .CountAsync(c => c.Kultura.IdKorisnik == idKorisnik);
        }

    }
}
