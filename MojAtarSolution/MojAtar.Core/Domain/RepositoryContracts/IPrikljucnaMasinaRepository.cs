using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IPrikljucnaMasinaRepository : IRepository<PrikljucnaMasina>
    {
        public Task<PrikljucnaMasina> GetByNazivIKorisnik(string naziv, Guid idKorisnik);
        public Task<bool> DeletePrikljucnaMasinaById(Guid? id);
        public Task<List<PrikljucnaMasina>> GetAllByKorisnik(Guid idKorisnik);
        Task<int> CountByKorisnikId(Guid korisnikId);
        Task<List<PrikljucnaMasina>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);
    }
}
