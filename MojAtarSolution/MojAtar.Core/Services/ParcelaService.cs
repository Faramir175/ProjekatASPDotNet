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
    public class ParcelaService : IParcelaService
    {
        private readonly IParcelaRepository _parcelaRepository;

        public ParcelaService(IParcelaRepository parcelaRepository)
        {
            _parcelaRepository = parcelaRepository;
        }

        public async Task<ParcelaDTO> Add(ParcelaDTO parcelaAdd)
        {
            if (parcelaAdd == null)
            {
                throw new ArgumentNullException(nameof(parcelaAdd));
            }

            if (parcelaAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(parcelaAdd.Naziv));
            }

            if (await _parcelaRepository.GetByNaziv(parcelaAdd.Naziv) != null)
            {
                throw new ArgumentException("Uneti naziv parcele vec postoji");
            }

            Parcela parcela = parcelaAdd.ToParcela();

            parcela.Id = Guid.NewGuid();

            await _parcelaRepository.Add(parcela);

            return parcela.ToParcelaDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Parcela? parcela = await _parcelaRepository.GetById(id.Value);
            if (parcela == null)
                return false;

            await _parcelaRepository.DeleteParcelaById(id.Value);

            return true;
        }

        public async Task<List<ParcelaDTO>> GetAllForUser(Guid idKorisnika)
        {
            List<Parcela> parcele = await _parcelaRepository.GetAllByKorisnik(idKorisnika);
            return parcele.Select(p => p.ToParcelaDTO()).ToList();
        }

        public async Task<ParcelaDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Parcela? parcela = await _parcelaRepository.GetById(id.Value);

            if (parcela == null) return null;

            return parcela.ToParcelaDTO();
        }
        public async Task<ParcelaDTO> GetByNaziv(string? naziv)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            Parcela? parcela = await _parcelaRepository.GetByNaziv(naziv);

            if (parcela == null) return null;

            return parcela.ToParcelaDTO();
        }

        public async Task<ParcelaDTO> Update(Guid? id, ParcelaDTO dto)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            Parcela? parcela = new Parcela()
            {
                Id = id.Value,
                BrojParcele = dto.BrojParcele,
                Naziv = dto.Naziv,
                Povrsina = dto.Povrsina,
                Napomena = dto.Napomena,
                IdKatastarskaOpstina = dto.IdKatastarskaOpstina,
                IdKorisnik = dto.IdKorisnik,
                Longitude = dto.Longitude,
                Latitude = dto.Latitude
            };

            await _parcelaRepository.Update(parcela);

            if (parcela == null) return null;
            return parcela.ToParcelaDTO();
        }
        public async Task<List<ParcelaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            var parcele = await _parcelaRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);
            return parcele.Select(p => p.ToParcelaDTO()).ToList();
        }


        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _parcelaRepository.GetCountByKorisnik(idKorisnik);
        }
        public async Task<List<ParcelaDTO>> GetAllByKorisnikPagedWithActiveKulture(Guid idKorisnik, int skip, int take)
        {
            return await _parcelaRepository.GetPagedWithActiveKulture(idKorisnik, skip, take);
        }

    }
}
