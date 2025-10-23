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
    public class PrikljucnaMasinaService : IPrikljucnaMasinaService
    {
        private readonly IPrikljucnaMasinaRepository _prikljucnaMasinaRepository;

        public PrikljucnaMasinaService(IPrikljucnaMasinaRepository prikljucnaMasinaRepository)
        {
            _prikljucnaMasinaRepository = prikljucnaMasinaRepository;
        }

        public async Task<PrikljucnaMasinaDTO> Add(PrikljucnaMasinaDTO prikljucnaMasinaAdd)
        {
            if (prikljucnaMasinaAdd == null)
            {
                throw new ArgumentNullException(nameof(prikljucnaMasinaAdd));
            }

            if (prikljucnaMasinaAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(prikljucnaMasinaAdd.Naziv));
            }

            if (await _prikljucnaMasinaRepository.GetByNaziv(prikljucnaMasinaAdd.Naziv) != null)
            {
                throw new ArgumentException("Uneti naziv prikljucne masine vec postoji");
            }

            PrikljucnaMasina prikljucnaMasina = prikljucnaMasinaAdd.ToPrikljucnaMasina();

            prikljucnaMasina.Id = Guid.NewGuid();

            await _prikljucnaMasinaRepository.Add(prikljucnaMasina);

            return prikljucnaMasina.ToPrikljucnaMasinaDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            PrikljucnaMasina? prikljucnaMasina = await _prikljucnaMasinaRepository.GetById(id.Value);
            if (prikljucnaMasina == null)
                return false;

            await _prikljucnaMasinaRepository.DeletePrikljucnaMasinaById(id.Value);

            return true;
        }

        public async Task<List<PrikljucnaMasinaDTO>> GetAllForUser(Guid idKorisnika)
        {
            List<PrikljucnaMasina> prikljucnaMasina = await _prikljucnaMasinaRepository.GetAllByKorisnik(idKorisnika);
            return prikljucnaMasina.Select(pm => pm.ToPrikljucnaMasinaDTO()).ToList();
        }



        public async Task<PrikljucnaMasinaDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            PrikljucnaMasina? prikljucnaMasina = await _prikljucnaMasinaRepository.GetById(id.Value);

            if (prikljucnaMasina == null) return null;

            return prikljucnaMasina.ToPrikljucnaMasinaDTO();
        }
        public async Task<PrikljucnaMasinaDTO> GetByNaziv(string? naziv)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            PrikljucnaMasina? prikljucnaMasina = await _prikljucnaMasinaRepository.GetByNaziv(naziv);

            if (prikljucnaMasina == null) return null;

            return prikljucnaMasina.ToPrikljucnaMasinaDTO();
        }

        public async Task<PrikljucnaMasinaDTO> Update(Guid? id, PrikljucnaMasinaDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            PrikljucnaMasina? prikljucnaMasina = new PrikljucnaMasina()
            {
                Id = id.Value,
                Naziv = dto.Naziv,
                TipMasine = dto.TipMasine,
                SirinaObrade = dto.SirinaObrade,
                PoslednjiServis = dto.PoslednjiServis,
                OpisServisa = dto.OpisServisa,
                IdKorisnik = dto.IdKorisnik
            };

            await _prikljucnaMasinaRepository.Update(prikljucnaMasina);

            if (prikljucnaMasina == null) return null;
            return prikljucnaMasina.ToPrikljucnaMasinaDTO();
        }
        public async Task<List<PrikljucnaMasinaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            var masine = await _prikljucnaMasinaRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);
            return masine.Select(pm => pm.ToPrikljucnaMasinaDTO()).ToList();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _prikljucnaMasinaRepository.GetCountByKorisnik(idKorisnik);
        }

    }
}
