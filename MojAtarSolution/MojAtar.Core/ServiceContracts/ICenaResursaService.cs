using MojAtar.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface ICenaResursaService
    {
        Task<List<CenaResursa>> GetPaged(Guid idKorisnik,int skip, int take);
        Task<int> GetTotalCount(Guid idKorisnik);
    }
}
