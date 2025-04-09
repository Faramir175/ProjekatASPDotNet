using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IRadnjaPrikljucnaMasinaService
    {
        Task<List<RadnjaPrikljucnaMasinaDTO>> GetAllByRadnjaId(Guid idRadnja);
        Task<RadnjaPrikljucnaMasinaDTO?> GetById(Guid idRadnja, Guid idPrikljucnaMasina);
        Task<RadnjaPrikljucnaMasinaDTO> Add(RadnjaPrikljucnaMasinaDTO dto);
        Task<RadnjaPrikljucnaMasinaDTO> Update(RadnjaPrikljucnaMasinaDTO dto);
        Task<bool> Delete(Guid idRadnja, Guid idPrikljucnaMasina);
        Task<List<RadnjaPrikljucnaMasinaDTO>> GetAllByUser(Guid idKorisnik);

    }
}
