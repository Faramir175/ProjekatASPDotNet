using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface ICenaKultureRepository
    {
        Task<List<CenaKulture>> GetPaged(Guid idKorisnik,int skip, int take);
        Task<int> GetTotalCount(Guid idKorisnik);
        Task<double> GetAktuelnaCena(Guid idKorisnik, Guid idKultura, DateTime datum);
        Task<DateTime?> GetDatumAktuelneCene(Guid idKultura, DateTime datum);

    }
}
