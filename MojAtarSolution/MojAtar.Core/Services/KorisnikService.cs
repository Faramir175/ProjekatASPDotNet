using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class KorisnikService:IKorisnikService
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikService(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
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
            Korisnik? korisnik = new Korisnik()
            {
                Id = id.Value,
                DatumRegistracije = (DateTime)dto.DatumRegistracije,
                Email = dto.Email,
                Ime = dto.Ime,
                Prezime = dto.Prezime,
                Parcele = dto.Parcele,
                Lozinka = dto.Lozinka,
                TipKorisnika = (Domain.Enums.KorisnikTip)dto.TipKorisnika
            };

            await _korisnikRepository.Update(korisnik);

            if (korisnik == null) return null;
            return korisnik.ToKorisnikResponse();
        }

    }
}
