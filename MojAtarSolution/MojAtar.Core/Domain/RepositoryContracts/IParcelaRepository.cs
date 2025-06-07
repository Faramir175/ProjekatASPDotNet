using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IParcelaRepository : IRepository<Parcela>
    {
        public Task<Parcela> GetByNaziv(string naziv);
        public Task<bool> DeleteParcelaById(Guid? id);
        public Task<List<Parcela>> GetAllByKorisnik(Guid idKorisnik);
        Task<int> CountByKorisnikId(Guid korisnikId);
    }
}
