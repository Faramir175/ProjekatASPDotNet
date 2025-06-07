using MojAtar.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IRadnjaRepository : IRepository<Radnja>
    {
        public Task<Radnja> GetByTipRadnje(RadnjaTip tipRadnje);
        public Task<bool> DeleteRadnjaById(Guid? id);
        public Task<List<Radnja>> GetAllByParcela(Guid idParcela);
        public Task<List<Radnja>> GetAllByKultura(Guid idKultura);
        public Task<decimal> GetUkupanPrinosZaParcelu(Guid idParcela);

        Task<int> GetCountByParcela(Guid idParcela);
        Task<int> GetCountByKorisnik(Guid idKorisnik);

        Task<List<Radnja>> GetAllByParcelaPaged(Guid idParcela, int skip, int take);
        Task<List<Radnja>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);

        Task<List<Radnja>> GetLastRadnjeByKorisnik(Guid korisnikId, int broj);
        Task<int> CountByKorisnikId(Guid korisnikId);
    }
}
