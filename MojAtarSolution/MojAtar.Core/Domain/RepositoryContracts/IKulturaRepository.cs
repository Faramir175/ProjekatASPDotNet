using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IKulturaRepository : IRepository<Kultura>
    {
        public Task<Kultura> GetByNazivIKorisnik(string naziv, Guid idKorisnik);
        public Task<bool> DeleteKulturaById(Guid? id);
        public Task<List<Kultura>> GetAllByKorisnik(Guid idKorisnik);
        public Task DodajCenu(CenaKulture cena);
        Task<int> CountByKorisnikId(Guid korisnikId);
        Task<List<Kultura>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);
    }
}
