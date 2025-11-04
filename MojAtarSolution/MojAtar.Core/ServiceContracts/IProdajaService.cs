using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IProdajaService
    {
        Task<List<ProdajaDTO>> GetAllByKorisnik(Guid korisnikId);
        Task Add(ProdajaDTO dto);
        Task Delete(Guid id);
        Task Update(ProdajaDTO dto);
        Task<List<ProdajaDTO>> GetPaged(Guid korisnikId, int skip, int take);
        Task<int> GetTotalCount(Guid korisnikId);
        Task<IzvestajProdajeResult?> GetIzvestajProdaje(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma);


        Task<ProdajaDTO?> GetById(Guid id);
        Task<decimal> GetUkupanPrinosZaKulturu(Guid idKultura);
        Task<decimal> GetUkupnoProdatoZaKulturu(Guid idKultura);
    }
}
