using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IRadnjaService
    {
        Task<RadnjaDTO> Add(RadnjaDTO dto);
        Task<RadnjaDTO> Update(Guid id, RadnjaDTO dto, List<Guid>? obrisaneRadneMasine = null, List<Guid>? obrisanePrikljucneMasine = null, List<Guid>? obrisaniResursi = null);
        Task<bool> DeleteById(Guid id);
        Task<RadnjaDTO> GetById(Guid id);
        Task<List<RadnjaDTO>> GetAllByParcela(Guid idParcela);
        Task<List<RadnjaDTO>> GetAll();
        Task<List<RadnjaDTO>> GetAllByKultura(Guid idKultura);
        Task<decimal> GetUkupanPrinosZaParcelu(Guid idParcela);
        Task<RadnjaDTO> GetByTipRadnje(RadnjaTip tip);
        Task<List<RadnjaDTO>> GetAllByParcelaPaged(Guid idParcela, int skip, int take);
        Task<int> GetCountByParcela(Guid idParcela);
        Task<List<RadnjaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);
        Task<RadnjaDTO> GetForEdit(Guid id, Guid idKorisnik);
        Task UpdateUkupanTrosak(Guid idRadnja);
        Task<decimal> GetSlobodnaPovrsinaAsync(Guid idParcela);


    }
}
