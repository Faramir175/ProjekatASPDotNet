using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IPrikljucnaMasinaService
    {
        public Task<PrikljucnaMasinaDTO> Add(PrikljucnaMasinaDTO dto);
        public Task<List<PrikljucnaMasinaDTO>> GetAllForUser(Guid idKorisnika);
        public Task<PrikljucnaMasinaDTO> GetById(Guid? id);
        public Task<PrikljucnaMasinaDTO> GetByNaziv(string? naziv);
        public Task<PrikljucnaMasinaDTO> Update(Guid? id, PrikljucnaMasinaDTO dto);
        public Task<bool> DeleteById(Guid? id);
        Task<List<PrikljucnaMasinaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take);
        Task<int> GetCountByKorisnik(Guid idKorisnik);

    }
}
