using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class KorisnikService:IKorisnikService
    {
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly IPasswordHasherService _passwordHasherService;

        public KorisnikService(IKorisnikRepository korisnikRepository, IPasswordHasherService passwordHasherService)
        {
            _korisnikRepository = korisnikRepository;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<KorisnikResponseDTO> Add(KorisnikRequestDTO korisnikAdd)
        {
            if (korisnikAdd == null)
            {
                throw new ArgumentNullException(nameof(korisnikAdd));
            }

            if (korisnikAdd.Email == null)
            {
                throw new ArgumentException(nameof(korisnikAdd.Email));
            }

            if (await _korisnikRepository.GetByEmail(korisnikAdd.Email) != null)
            {
                throw new ArgumentException("Given email already exists");
            }

            Korisnik korisnik = korisnikAdd.ToKorisnik();

            korisnik.Id = Guid.NewGuid();

            await _korisnikRepository.Add(korisnik);

            return korisnik.ToKorisnikResponse();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Korisnik? korisnik = await _korisnikRepository.GetById(id.Value);
            if (korisnik == null)
                return false;

            await _korisnikRepository.DeleteKorisnikById(id.Value);

            return true;
        }

        public async Task<bool> DeleteLoggedInUser(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            Korisnik? korisnik = await _korisnikRepository.GetByEmail(email);

            if (korisnik == null || korisnik.Id == null) return false;

            return await DeleteById(korisnik.Id.Value);
        }

        public async Task<List<KorisnikResponseDTO>> GetAll()
        {
            List<Korisnik> korisnici = await _korisnikRepository.GetAll();
            List<KorisnikResponseDTO> korisniciR = new List<KorisnikResponseDTO>();
            foreach(Korisnik k in korisnici)
            {
                korisniciR.Add(k.ToKorisnikResponse());
            }
            return korisniciR;
        }

        public async Task<KorisnikResponseDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Korisnik? korisnik = await _korisnikRepository.GetById(id.Value);

            if (korisnik == null) return null;

            return korisnik.ToKorisnikResponse();
        }
        public async Task<KorisnikResponseDTO> GetByEmail(string? email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            Korisnik? korisnik = await _korisnikRepository.GetByEmail(email);

            if (korisnik == null) return null;

            return korisnik.ToKorisnikResponse();
        }

        public async Task<KorisnikResponseDTO> Update(Guid? id, KorisnikRequestDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Korisnik? originalniKorisnikDomain = await _korisnikRepository.GetById(id.Value);
            if (originalniKorisnikDomain == null)
            {
                throw new ArgumentException($"Korisnik sa ID-em {id.Value} ne postoji.");
            }
            KorisnikResponseDTO originalniKorisnik = originalniKorisnikDomain.ToKorisnikResponse();

            // 2. Logika popunjavanja nedostajućih polja (premeštena iz kontrolera)
            // Ako polje u dolaznom DTO-u (dto) nije popunjeno, uzmi vrednost iz originalnogKorisnika.
            dto.Ime = string.IsNullOrEmpty(dto.Ime) ? originalniKorisnik.Ime : dto.Ime;
            dto.Prezime = string.IsNullOrEmpty(dto.Prezime) ? originalniKorisnik.Prezime : dto.Prezime;
            dto.Email = string.IsNullOrEmpty(dto.Email) ? originalniKorisnik.Email : dto.Email;
            dto.TipKorisnika = dto.TipKorisnika ?? originalniKorisnik.TipKorisnika;
            dto.DatumRegistracije = dto.DatumRegistracije ?? originalniKorisnik.DatumRegistracije;
            dto.Parcele = dto.Parcele ?? originalniKorisnik.Parcele;

            if (string.IsNullOrEmpty(dto.Lozinka))
            {
                dto.Lozinka = originalniKorisnik.Lozinka;
            }
            else
            {
                dto.Lozinka = _passwordHasherService.HashPassword(dto.Lozinka);
            }

            Korisnik korisnikZaUpdate = dto.ToKorisnik();
            korisnikZaUpdate.Id = id.Value;

            await _korisnikRepository.Update(korisnikZaUpdate);

            return korisnikZaUpdate.ToKorisnikResponse();
        }

        public async Task<(KorisnikResponseDTO?, List<Claim>?)> AzurirajKorisnikaSaVerifikacijom(
            Guid korisnikId,
            string trenutnaLozinka,
            KorisnikRequestDTO noviPodaci)
        {
            Korisnik? logovaniKorisnikDomain = await _korisnikRepository.GetById(korisnikId);

            if (logovaniKorisnikDomain == null)
            {
                throw new ArgumentException($"Korisnik sa ID-em {korisnikId} ne postoji.");
            }

            bool isPasswordValid = _passwordHasherService.VerifyPassword(logovaniKorisnikDomain.Lozinka, trenutnaLozinka);

            if (isPasswordValid)
            {
                KorisnikResponseDTO updatedKorisnik = await Update(korisnikId, noviPodaci);

                List<Claim> noviClaims = GenerateClaims(updatedKorisnik);

                return (updatedKorisnik, noviClaims);
            }

            return (null, null);
        }
        public async Task<KorisnikResponseDTO> RegisterNewUser(KorisnikRequestDTO korisnikRequest)
        {
            if (korisnikRequest == null)
            {
                throw new ArgumentNullException(nameof(korisnikRequest));
            }

            if (korisnikRequest.Email == null)
            {
                throw new ArgumentException("Email je obavezan.", nameof(korisnikRequest.Email));
            }

            if (await _korisnikRepository.GetByEmail(korisnikRequest.Email) != null)
            {
                throw new ArgumentException("Korisnik sa datim emailom već postoji.");
            }

            string heshovanaLozinka = _passwordHasherService.HashPassword(korisnikRequest.Lozinka);
            korisnikRequest.Lozinka = heshovanaLozinka; 

            Korisnik korisnik = korisnikRequest.ToKorisnik();
            korisnik.Id = Guid.NewGuid();

            await _korisnikRepository.Add(korisnik);

            return korisnik.ToKorisnikResponse();
        }

        public async Task<KorisnikResponseDTO?> Authenticate(string email, string lozinka)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(lozinka))
            {
                return null;
            }

            Korisnik? korisnik = await _korisnikRepository.GetByEmail(email);

            if (korisnik == null)
            {
                return null; 
            }

            var dummyRequest = new KorisnikRequestDTO { Email = email };

            bool isPasswordValid = _passwordHasherService.VerifyPassword(
                        korisnik.Lozinka, 
                        lozinka);

            if (isPasswordValid)
            {
                return korisnik.ToKorisnikResponse();
            }
            else
            {
                return null;
            }
        }
        public List<Claim> GenerateClaims(KorisnikResponseDTO korisnik)
        {
            if (korisnik == null)
            {
                throw new ArgumentNullException(nameof(korisnik));
            }

            var claims = new List<Claim>
             {
                 new Claim(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),
                 new Claim(ClaimTypes.Name, korisnik.Email),
                 new Claim(ClaimTypes.Role, korisnik.TipKorisnika.ToString()),
                 new Claim("Ime", korisnik.Ime),
                 new Claim("Prezime", korisnik.Prezime),
                 new Claim("DatumRegistracije", korisnik.DatumRegistracije.ToString("o"))
             };

            return claims;
        }

        public Guid? GetKorisnikIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }
            return null;
        }
    }
}
