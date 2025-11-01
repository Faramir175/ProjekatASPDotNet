using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IParcelaRepository : IRepository<Parcela>
    {
        public Task<Parcela> GetByNazivIKorisnik(string naziv, Guid idKorisnik);
        public Task<bool> DeleteParcelaById(Guid? id);
        public Task<List<Parcela>> GetAllByKorisnik(Guid idKorisnik);
        Task<int> CountByKorisnikId(Guid korisnikId);
        Task<List<ParcelaDTO>> GetPagedWithActiveKulture(Guid idKorisnik, int skip, int take);

        Task<int> GetCountByKorisnik(Guid idKorisnik);
        Task<List<Parcela>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);

    }
}
