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

        public async Task<List<CenaResursa>> GetPaged(Guid idKorisnik, int skip, int take)
        {
            return await _repo.GetPaged(idKorisnik, skip, take);
        }

        public async Task<int> GetTotalCount(Guid idKorisnik)
        {
            return await _repo.GetTotalCount(idKorisnik);
        }

        public async Task<double> GetAktuelnaCena(Guid idKorisnik, Guid idResurs, DateTime datum)
        {
            return await _repo.GetAktuelnaCena(idKorisnik, idResurs, datum);
        }
        public async Task<DateTime?> GetDatumAktuelneCene(Guid idResurs, DateTime datum)
        {
            return await _repo.GetDatumAktuelneCene(idResurs, datum);
        }

    }
}
