using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.DTO.Extensions;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class RadnaMasinaService : IRadnaMasinaService
    {
        private readonly IRadnaMasinaRepository _radnaMasinaRepository;

        public RadnaMasinaService(IRadnaMasinaRepository radnaMasinaRepository)
        {
            _radnaMasinaRepository = radnaMasinaRepository;
        }

        public async Task<RadnaMasinaDTO> Add(RadnaMasinaDTO radnaMasinaAdd)
        {
            if (radnaMasinaAdd == null)
            {
                throw new ArgumentNullException(nameof(radnaMasinaAdd));
            }

            if (radnaMasinaAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(radnaMasinaAdd.Naziv));
            }

            if (await _radnaMasinaRepository.GetByNazivIKorisnik(radnaMasinaAdd.Naziv, radnaMasinaAdd.IdKorisnik) != null)
            {
                throw new ArgumentException("Uneti naziv parcele vec postoji");
            }

            RadnaMasina radnaMasina = radnaMasinaAdd.ToRadnaMasina();

            radnaMasina.Id = Guid.NewGuid();

            await _radnaMasinaRepository.Add(radnaMasina);

            return radnaMasina.ToRadnaMasinaDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            RadnaMasina? radnaMasina = await _radnaMasinaRepository.GetById(id.Value);
            if (radnaMasina == null)
                return false;

            await _radnaMasinaRepository.DeleteRadnaMasinaById(id.Value);

            return true;
        }

        public async Task<List<RadnaMasinaDTO>> GetAllForUser(Guid idKorisnika)
        {
            List<RadnaMasina> radnaMasina = await _radnaMasinaRepository.GetAllByKorisnik(idKorisnika);
            return radnaMasina.Select(rm => rm.ToRadnaMasinaDTO()).ToList();
        }



        public async Task<RadnaMasinaDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            RadnaMasina? radnaMasina = await _radnaMasinaRepository.GetById(id.Value);

            if (radnaMasina == null) return null;

            return radnaMasina.ToRadnaMasinaDTO();
        }
        public async Task<RadnaMasinaDTO> GetByNaziv(string? naziv, Guid idKorisnik)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            RadnaMasina? radnaMasina = await _radnaMasinaRepository.GetByNazivIKorisnik(naziv, idKorisnik);

            if (radnaMasina == null) return null;

            return radnaMasina.ToRadnaMasinaDTO();
        }

        public async Task<RadnaMasinaDTO> Update(Guid? id, RadnaMasinaDTO dto)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            // 1. DOHVATI POSTOJEĆU
            var stara = await _radnaMasinaRepository.GetById(id.Value);
            if (stara == null) return null;

            // 2. PROVERA DUPLIKATA (Samo ako se naziv menja)
            if (!string.Equals(stara.Naziv, dto.Naziv, StringComparison.OrdinalIgnoreCase))
            {
                var postoji = await _radnaMasinaRepository.GetByNazivIKorisnik(dto.Naziv, dto.IdKorisnik);
                if (postoji != null && postoji.Id != id)
                    throw new ArgumentException("Već postoji radna mašina sa ovim nazivom za vaš nalog.");
            }

            // 3. IZMENA POLJA NA POSTOJEĆEM OBJEKTU
            stara.Naziv = dto.Naziv;
            stara.TipUlja = dto.TipUlja;
            stara.RadniSatiServis = (int)dto.RadniSatiServis;
            stara.PoslednjiServis = dto.PoslednjiServis;
            stara.OpisServisa = dto.OpisServisa;
            stara.UkupanBrojRadnihSati = (int)dto.UkupanBrojRadnihSati;
            // stara.IdKorisnik = dto.IdKorisnik; // Nije obavezno menjati vlasnika

            // 4. POZIV UPDATE
            await _radnaMasinaRepository.Update(stara);

            return stara.ToRadnaMasinaDTO();
        }


        public async Task<List<RadnaMasinaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            var masine = await _radnaMasinaRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);
            return masine.Select(rm => rm.ToRadnaMasinaDTO()).ToList();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _radnaMasinaRepository.GetCountByKorisnik(idKorisnik);
        }
    }
}
