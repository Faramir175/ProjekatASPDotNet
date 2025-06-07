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
        Task<int> GetBrojParcelaAsync(Guid korisnikId);
        Task<int> GetBrojRadnjiAsync(Guid korisnikId);
        Task<int> GetBrojResursaAsync(Guid korisnikId);
        Task<int> GetBrojRadnihMasinaAsync(Guid korisnikId);
        Task<int> GetBrojPrikljucnihMasinaAsync(Guid korisnikId);
        Task<int> GetBrojKulturaAsync(Guid korisnikId);
        Task<List<Radnja>> GetPoslednjeRadnjeAsync(Guid korisnikId, int broj = 3);
    }
}
