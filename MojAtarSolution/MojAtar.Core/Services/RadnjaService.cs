using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
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
    public class RadnjaService : IRadnjaService
    {
        private readonly IRadnjaRepository _radnjaRepository;

        public RadnjaService(IRadnjaRepository radnjaRepository)
        {
            _radnjaRepository = radnjaRepository;
        }

        public async Task<RadnjaDTO> Add(RadnjaDTO dto)
        {
            Radnja novaRadnja;

            // Ako je u ComboBox-u izabrana Zetva
            if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                novaRadnja = new Zetva
                {
                    DatumIzvrsenja = dto.DatumIzvrsenja,
                    TipRadnje = dto.TipRadnje,
                    IdParcela = dto.IdParcela,
                    IdKultura = dto.IdKultura,
                    Napomena = dto.Napomena,
                    UkupanTrosak = dto.UkupanTrosak,
                    VremenskiUslovi = dto.VremenskiUslovi,
                    Prinos = (double)dto.Prinos // <--- samo za žetvu
                };
            }
            else
            {
                // Sve ostale radnje
                novaRadnja = new Radnja
                {
                    DatumIzvrsenja = dto.DatumIzvrsenja,
                    TipRadnje = dto.TipRadnje,
                    IdParcela = dto.IdParcela,
                    IdKultura = dto.IdKultura,
                    Napomena = dto.Napomena,
                    UkupanTrosak = dto.UkupanTrosak,
                    VremenskiUslovi = dto.VremenskiUslovi
                };
            }

            // Snimanje u bazu
            var entity = await _radnjaRepository.Add(novaRadnja);
            return entity.ToRadnjaDTO();
        }


        public async Task<RadnjaDTO> Update(Guid id, RadnjaDTO dto)
        {
            var radnja = dto.ToRadnja();
            radnja.Id = id;
            var updated = await _radnjaRepository.Update(radnja);
            return updated.ToRadnjaDTO();
        }

        public async Task<bool> DeleteById(Guid id)
        {
            var radnja = await _radnjaRepository.GetById(id);
            if (radnja == null) return false;

            return await _radnjaRepository.Delete(radnja);
        }

        public async Task<RadnjaDTO> GetById(Guid id)
        {
            var radnja = await _radnjaRepository.GetById(id);
            return radnja?.ToRadnjaDTO();
        }

        public async Task<List<RadnjaDTO>> GetAllByParcela(Guid idParcela)
        {
            var radnje = await _radnjaRepository.GetAllByParcela(idParcela);
            return radnje.Select(r => r.ToRadnjaDTO()).ToList();
        }

        public async Task<List<RadnjaDTO>> GetAll()
        {
            var radnje = await _radnjaRepository.GetAll();
            return radnje.Select(r => r.ToRadnjaDTO()).ToList();
        }

        public async Task<List<RadnjaDTO>> GetAllByKultura(Guid idKultura)
        {
            var radnje = await _radnjaRepository.GetAllByKultura(idKultura);
            return radnje.Select(r => r.ToRadnjaDTO()).ToList();
        }

        public async Task<decimal> GetUkupanPrinosZaParcelu(Guid idParcela)
        {
            return await _radnjaRepository.GetUkupanPrinosZaParcelu(idParcela);
        }

        public async Task<RadnjaDTO> GetByTipRadnje(RadnjaTip tip)
        {
            var radnja = await _radnjaRepository.GetByTipRadnje(tip);
            return radnja?.ToRadnjaDTO();
        }

        public async Task<List<RadnjaDTO>> GetAllByParcelaPaged(Guid idParcela, int skip, int take)
        {
            var radnje = await _radnjaRepository.GetAllByParcelaPaged(idParcela, skip, take);
            return radnje.Select(x => x.ToRadnjaDTO()).ToList();
        }

        public async Task<int> GetCountByParcela(Guid idParcela)
        {
            return await _radnjaRepository.GetCountByParcela(idParcela);
        }

        public async Task<List<RadnjaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            var radnje = await _radnjaRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);
            return radnje.Select(x => x.ToRadnjaDTO()).ToList();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _radnjaRepository.GetCountByKorisnik(idKorisnik);
        }

    }
}
