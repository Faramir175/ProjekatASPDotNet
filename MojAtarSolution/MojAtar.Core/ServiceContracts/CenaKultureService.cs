using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public class CenaKultureService : ICenaKultureService
    {
        private readonly ICenaKultureRepository _repo;

        public CenaKultureService(ICenaKultureRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CenaKulture>> GetPaged(int skip, int take)
        {
            return await _repo.GetPaged(skip, take);
        }

        public async Task<int> GetTotalCount()
        {
            return await _repo.GetTotalCount();
        }
    }
}
