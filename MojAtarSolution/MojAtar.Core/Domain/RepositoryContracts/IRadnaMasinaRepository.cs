using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRadnaMasinaRepository : IRepository<RadnaMasina>
    {
        public Task<RadnaMasina> GetByNaziv(string naziv);
        public Task<bool> DeleteRadnaMasinaById(Guid? id);
        public Task<List<RadnaMasina>> GetAllByKorisnik(Guid idKorisnik);

    }
}
