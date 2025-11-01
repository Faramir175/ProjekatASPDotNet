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
        private readonly IParcelaKulturaService _parcelaKulturaService;

        public RadnjaService(IRadnjaRepository radnjaRepository,IParcelaKulturaService parcelaKulturaService)
        {
            _radnjaRepository = radnjaRepository;
            _parcelaKulturaService = parcelaKulturaService;
        }

        public async Task<RadnjaDTO> Add(RadnjaDTO dto)
        {
            // 1️⃣ Validacija SETVE
            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                var parcela = await _radnjaRepository.GetParcelaSaSetvama((Guid)dto.IdParcela);
                if (parcela == null)
                    throw new Exception("Parcela nije pronađena.");

                var nezavrseneSetve = parcela.ParceleKulture
                    .Where(pk => pk.DatumZetve == null)
                    .ToList();

                var setveZaRacunanje = nezavrseneSetve
                    .Where(pk => !(pk.IdKultura == dto.IdKultura
                                   && pk.Povrsina == dto.Povrsina
                                   && pk.DatumSetve == dto.DatumIzvrsenja))
                    .ToList();


                decimal slobodno = parcela.Povrsina - setveZaRacunanje.Sum(pk => pk.Povrsina);

                if (dto.Povrsina > slobodno)
                {
                    await _parcelaKulturaService.DeleteIfNotCompleted(
                        (Guid)dto.IdParcela,
                        (Guid)dto.IdKultura,
                        dto.DatumIzvrsenja
                    );
                    throw new Exception($"Nema dovoljno slobodne površine. Dostupno: {slobodno:F2} ha.");
                }

            }
            else if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                // ✅ Tražimo samo setvu koja još NEMA DatumZetve (nezavršena)
                var nezavrsenaSetva = await _parcelaKulturaService.GetNezavrsenaSetva(
                    (Guid)dto.IdParcela,
                    (Guid)dto.IdKultura
                );

                if (nezavrsenaSetva == null)
                    throw new Exception("Nema aktivne (nezavršene) setve za ovu kulturu na parceli.");

                nezavrsenaSetva.DatumZetve = dto.DatumIzvrsenja;
                await _parcelaKulturaService.Update(nezavrsenaSetva);
            }

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
                };
            }

            // Snimanje u bazu
            var entity = await _radnjaRepository.Add(novaRadnja);
            return entity.ToRadnjaDTO();
        }


        public async Task<RadnjaDTO> Update(Guid id, RadnjaDTO dto)
        {
            var radnja = await _radnjaRepository.GetById(id);
            if (radnja == null)
                throw new Exception("Radnja nije pronađena.");

            // 1️⃣ Ažuriraj osnovne podatke radnje
            radnja.DatumIzvrsenja = dto.DatumIzvrsenja;
            radnja.Napomena = dto.Napomena;
            radnja.UkupanTrosak = dto.UkupanTrosak;

            // 2️⃣ Update Povrsine za setvu sa validacijom
            if (radnja.TipRadnje == RadnjaTip.Setva)
            {
                var parcela = await _radnjaRepository.GetParcelaSaSetvama(radnja.IdParcela.Value);
                if (parcela == null)
                    throw new Exception("Parcela nije pronađena.");

                var parcelaKultura = await _parcelaKulturaService
                    .GetByParcelaAndKulturaId(radnja.IdParcela.Value, radnja.IdKultura.Value);

                // Izračunaj trenutno zauzetu površinu bez ove setve
                decimal trenutnoZauzeto = parcela.ParceleKulture
                    .Where(pk => pk.DatumZetve == null && pk.Id != parcelaKultura.Id)
                    .Sum(pk => pk.Povrsina);

                decimal slobodno = parcela.Povrsina - trenutnoZauzeto;

                if (dto.Povrsina > slobodno)
                    throw new Exception($"Nema dovoljno slobodne površine. Dostupno: {slobodno:F4} ha.");

                // Update površine
                parcelaKultura.Povrsina = dto.Povrsina ?? parcelaKultura.Povrsina;
                await _parcelaKulturaService.Update(parcelaKultura);
            }
            else if (radnja.TipRadnje == RadnjaTip.Zetva)
            {
                var parcelaKultura = await _parcelaKulturaService
                    .GetByParcelaAndKulturaId(radnja.IdParcela.Value, radnja.IdKultura.Value);

                parcelaKultura.DatumZetve = dto.DatumIzvrsenja;
                await _parcelaKulturaService.Update(parcelaKultura);
            }

            // 3️⃣ Update radnje
            var updated = await _radnjaRepository.Update(radnja);
            return updated.ToRadnjaDTO();
        }

        public async Task<bool> DeleteById(Guid id)
        {
            var radnja = await _radnjaRepository.GetById(id);
            if (radnja == null) return false;

            if (radnja.TipRadnje == RadnjaTip.Setva)
            {
                await _parcelaKulturaService.DeleteIfNotCompleted((Guid)radnja.IdParcela,(Guid)radnja.IdKultura,radnja.DatumIzvrsenja
                );
            }
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
