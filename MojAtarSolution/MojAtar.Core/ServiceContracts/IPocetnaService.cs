using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IPocetnaService
    {
        Task<PocetnaDTO> GetDashboardDataAsync(Guid korisnikId);
    }
}
