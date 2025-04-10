using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IRadnjaResursService
    {
        Task<List<RadnjaResursDTO>> GetAllByRadnjaId(Guid idRadnja);
        Task<RadnjaResursDTO?> GetById(Guid idRadnja, Guid idResurs);
        Task<RadnjaResursDTO> Add(RadnjaResursDTO dto);
        Task<RadnjaResursDTO> Update(RadnjaResursDTO dto);
        Task<bool> Delete(Guid idRadnja, Guid idResurs);
        Task<List<RadnjaResursDTO>> GetAllByUser(Guid idKorisnik);

    }
}
