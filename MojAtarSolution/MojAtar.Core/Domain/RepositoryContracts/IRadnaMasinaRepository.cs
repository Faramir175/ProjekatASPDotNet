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
        public Task<RadnaMasina> GetByNazivIKorisnik(string naziv, Guid idKorisnik);
        public Task<bool> DeleteRadnaMasinaById(Guid? id);
        public Task<List<RadnaMasina>> GetAllByKorisnik(Guid idKorisnik);
        Task<int> CountByKorisnikId(Guid korisnikId);
        Task<List<RadnaMasina>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);
    }
}
