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
        public Task<PrikljucnaMasina> GetByNaziv(string naziv);
        public Task<bool> DeletePrikljucnaMasinaById(Guid? id);
        public Task<List<PrikljucnaMasina>> GetAllByKorisnik(Guid idKorisnik);
        Task<int> CountByKorisnikId(Guid korisnikId);
        Task<List<PrikljucnaMasina>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);
    }
}
