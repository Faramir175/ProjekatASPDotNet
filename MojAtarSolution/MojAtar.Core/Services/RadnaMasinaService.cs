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

            if (await _radnaMasinaRepository.GetByNaziv(radnaMasinaAdd.Naziv) != null)
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
        public async Task<RadnaMasinaDTO> GetByNaziv(string? naziv)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            RadnaMasina? radnaMasina = await _radnaMasinaRepository.GetByNaziv(naziv);

            if (radnaMasina == null) return null;

            return radnaMasina.ToRadnaMasinaDTO();
        }

        public async Task<RadnaMasinaDTO> Update(Guid? id, RadnaMasinaDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            RadnaMasina? radnaMasina = new RadnaMasina()
            {
                Id = id.Value,
                Naziv = dto.Naziv,
                TipUlja = dto.TipUlja,
                RadniSatiServis = dto.RadniSatiServis,
                PoslednjiServis = dto.PoslednjiServis,
                OpisServisa = dto.OpisServisa,
                UkupanBrojRadnihSati = dto.UkupanBrojRadnihSati,
                IdKorisnik = dto.IdKorisnik
            };

            await _radnaMasinaRepository.Update(radnaMasina);

            if (radnaMasina == null) return null;
            return radnaMasina.ToRadnaMasinaDTO();
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
