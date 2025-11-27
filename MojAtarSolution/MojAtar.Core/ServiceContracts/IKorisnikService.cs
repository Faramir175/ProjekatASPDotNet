using MojAtar.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.ServiceContracts
{
    public interface IKorisnikService
    {
        Task<KorisnikResponseDTO> Add(KorisnikRequestDTO dto);
        Task<List<KorisnikResponseDTO>> GetAll();
        Task<KorisnikResponseDTO> GetById(Guid? id);
        Task<KorisnikResponseDTO> GetByEmail(string? email);
        Task<KorisnikResponseDTO> Update(Guid? id, KorisnikRequestDTO dto);
        Task<(KorisnikResponseDTO?, List<Claim>?)> AzurirajKorisnikaSaVerifikacijom(Guid korisnikId, string trenutnaLozinka, KorisnikRequestDTO noviPodaci);
        Task<bool> DeleteById(Guid? id);
        Task<bool> DeleteLoggedInUser(string email);
        Task<KorisnikResponseDTO> RegisterNewUser(KorisnikRequestDTO korisnikRequest);
        Task<KorisnikResponseDTO?> Authenticate(string email, string lozinka);
        List<Claim> GenerateClaims(KorisnikResponseDTO korisnik);
    }
}
