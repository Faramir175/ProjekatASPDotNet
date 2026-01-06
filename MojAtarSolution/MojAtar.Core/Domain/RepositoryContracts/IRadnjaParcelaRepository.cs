using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRadnjaParcelaRepository : IRepository<RadnjaParcela>
    {
        Task<List<RadnjaParcela>> GetAllByRadnjaId(Guid idRadnja);
        Task<RadnjaParcela?> GetByRadnjaAndParcela(Guid idRadnja, Guid idParcela);
    }
}
