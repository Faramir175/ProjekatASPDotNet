using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Domain.RepositoryContracts
{
    public interface IProdajaRepository
    {
        Task<List<Prodaja>> GetAllByKorisnik(Guid korisnikId);
        Task<Prodaja?> GetById(Guid id);
        Task<Prodaja> Add(Prodaja prodaja);
        Task Delete(Guid id);
        Task<Prodaja> Update(Prodaja prodaja);
        Task<Kultura?> GetKulturaById(Guid idKultura);
        Task<List<Prodaja>> GetPaged(Guid korisnikId, int skip, int take);
        Task<int> GetTotalCount(Guid korisnikId);
        Task<List<Prodaja>> GetByKorisnikAndPeriod(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma);
        Task<Dictionary<Guid, decimal>> GetPrinosPoKulturi(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma);


        // Pomoćne metode
        Task<decimal> GetUkupanPrinosZaKulturu(Guid idKultura);
        Task<decimal> GetUkupnoProdatoZaKulturu(Guid idKultura);
    }
}
