using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface ICenaResursaRepository
    {
        Task<List<CenaResursa>> GetPaged(int skip, int take);
        Task<int> GetTotalCount();
    }
}
