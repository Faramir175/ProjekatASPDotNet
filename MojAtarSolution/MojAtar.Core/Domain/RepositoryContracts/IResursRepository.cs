using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IResursRepository : IRepository<Resurs>
    {
        public Task<Resurs> GetByNazivIKorisnik(string naziv, Guid idKorisnik);
        public Task<bool> DeleteResursById(Guid? id);
        public Task<List<Resurs>> GetAllByKorisnik(Guid idKorisnik);
        public Task DodajCenu(CenaResursa cena);
        Task<int> CountByKorisnikId(Guid korisnikId);
        Task<List<Resurs>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);

    }
}
