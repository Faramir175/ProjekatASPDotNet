using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface ICenaResursaRepository
    {
        Task<List<CenaResursa>> GetPaged(Guid idKorisnik,int skip, int take);
        Task<int> GetTotalCount(Guid idKorisnik);
        Task<double> GetAktuelnaCena(Guid idKorisnik, Guid idResurs, DateTime datum);
    }
}
