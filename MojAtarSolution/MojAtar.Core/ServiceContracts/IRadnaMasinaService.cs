using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IRadnaMasinaService
    {
        public Task<RadnaMasinaDTO> Add(RadnaMasinaDTO dto);
        public Task<List<RadnaMasinaDTO>> GetAllForUser(Guid idKorisnika);
        public Task<RadnaMasinaDTO> GetById(Guid? id);
        public Task<RadnaMasinaDTO> GetByNaziv(string? naziv, Guid idKorisnik);
        public Task<RadnaMasinaDTO> Update(Guid? id, RadnaMasinaDTO dto);
        public Task<bool> DeleteById(Guid? id);
        Task<List<RadnaMasinaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);

    }
}
