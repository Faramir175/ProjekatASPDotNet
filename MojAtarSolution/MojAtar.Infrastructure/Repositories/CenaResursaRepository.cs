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

        public async Task<List<CenaResursa>> GetPaged(int skip, int take)
        {
            return await _dbContext.CeneResursa
                .Include(c => c.Resurs)
                .OrderByDescending(c => c.DatumVaznosti)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetTotalCount()
        {
            return await _dbContext.CeneResursa.CountAsync();
        }
    }
}
