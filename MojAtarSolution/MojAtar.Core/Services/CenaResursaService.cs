using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class CenaResursaService : ICenaResursaService
    {
        private readonly ICenaResursaRepository _repo;

        public CenaResursaService(ICenaResursaRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CenaResursa>> GetPaged(int skip, int take)
        {
            return await _repo.GetPaged(skip, take);
        }

        public async Task<int> GetTotalCount()
        {
            return await _repo.GetTotalCount();
        }
    }
}
