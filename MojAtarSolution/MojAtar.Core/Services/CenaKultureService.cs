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
    public class CenaKultureService : ICenaKultureService
    {
        private readonly ICenaKultureRepository _repo;

        public CenaKultureService(ICenaKultureRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CenaKulture>> GetPaged(Guid idKorisnik, int skip, int take)
        {
            return await _repo.GetPaged(idKorisnik, skip, take);
        }

        public async Task<int> GetTotalCount(Guid idKorisnik)
        {
            return await _repo.GetTotalCount(idKorisnik);
        }
    }
}
