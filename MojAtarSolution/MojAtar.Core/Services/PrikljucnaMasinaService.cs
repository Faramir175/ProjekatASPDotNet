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
                throw new ArgumentNullException(nameof(prikljucnaMasinaAdd));

            if (string.IsNullOrWhiteSpace(prikljucnaMasinaAdd.Naziv))
                throw new ArgumentException(nameof(prikljucnaMasinaAdd.Naziv));

            var existing = await _prikljucnaMasinaRepository.GetByNazivIKorisnik(
                prikljucnaMasinaAdd.Naziv,
                prikljucnaMasinaAdd.IdKorisnik
            );

            if (existing != null)
                throw new ArgumentException("Već postoji priključna mašina sa ovim nazivom za vaš nalog.");

            var prikljucnaMasina = prikljucnaMasinaAdd.ToPrikljucnaMasina();
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
        public async Task<PrikljucnaMasinaDTO> GetByNaziv(string? naziv, Guid idKorisnik)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            PrikljucnaMasina? prikljucnaMasina = await _prikljucnaMasinaRepository.GetByNazivIKorisnik(naziv,idKorisnik);

            if (prikljucnaMasina == null) return null;

            return prikljucnaMasina.ToPrikljucnaMasinaDTO();
        }

        public async Task<PrikljucnaMasinaDTO> Update(Guid? id, PrikljucnaMasinaDTO dto)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var stara = await _prikljucnaMasinaRepository.GetById(id.Value);
            if (stara == null)
                return null;

            // Provera duplikata — ako postoji druga mašina sa istim nazivom
            if (!string.Equals(stara.Naziv, dto.Naziv, StringComparison.OrdinalIgnoreCase))
            {
                var postoji = await _prikljucnaMasinaRepository.GetByNazivIKorisnik(dto.Naziv, dto.IdKorisnik);
                if (postoji != null && postoji.Id != id)
                    throw new ArgumentException("Već postoji priključna mašina sa ovim nazivom za vaš nalog.");
            }

            stara.Naziv = dto.Naziv;
            stara.TipMasine = dto.TipMasine;
            stara.SirinaObrade = (double)dto.SirinaObrade;
            stara.PoslednjiServis = dto.PoslednjiServis;
            stara.OpisServisa = dto.OpisServisa;
            stara.IdKorisnik = dto.IdKorisnik;

            await _prikljucnaMasinaRepository.Update(stara);
            return stara.ToPrikljucnaMasinaDTO();
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
