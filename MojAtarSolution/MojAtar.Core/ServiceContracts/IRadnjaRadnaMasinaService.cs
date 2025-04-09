using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IRadnjaRadnaMasinaService
    {
        Task<List<RadnjaRadnaMasinaDTO>> GetAllByRadnjaId(Guid idRadnja);
        Task<RadnjaRadnaMasinaDTO?> GetById(Guid idRadnja, Guid idRadnaMasina);
        Task<RadnjaRadnaMasinaDTO> Add(RadnjaRadnaMasinaDTO dto);
        Task<RadnjaRadnaMasinaDTO> Update(RadnjaRadnaMasinaDTO dto);
        Task<bool> Delete(Guid idRadnja, Guid idRadnaMasina);
        Task<List<RadnjaRadnaMasinaDTO>> GetAllByUser(Guid idKorisnik);

    }
}
