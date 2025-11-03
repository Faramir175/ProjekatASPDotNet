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
            if (dto.IdParcela == null || dto.IdKultura == null)
                throw new Exception("Parcela i kultura su obavezni.");

            //  Validacija SETVE
            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                var parcela = await _radnjaRepository.GetParcelaSaSetvama(dto.IdParcela.Value);
                if (parcela == null)
                    throw new Exception("Parcela nije pronađena.");

                // proveri zauzetost
                var zauzete = parcela.ParceleKulture
                    .Where(pk => pk.IdZetvaRadnja == null) // aktivne setve
                    .Sum(pk => pk.Povrsina);

                var slobodno = parcela.Povrsina - zauzete;
                if (dto.Povrsina > slobodno)
                    throw new Exception($"Nema dovoljno slobodne površine. Dostupno: {slobodno:F2} ha.");
            }

            //  Validacija ŽETVE
            if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                var aktivnaSetva = await _parcelaKulturaService.GetNezavrsenaSetva(
                    dto.IdParcela.Value, dto.IdKultura.Value);

                if (aktivnaSetva == null)
                    throw new Exception("Nema aktivne setve za ovu kulturu na parceli.");
            }

            //  1. Kreiraj radnju
            Radnja novaRadnja = dto.TipRadnje == RadnjaTip.Zetva
                ? new Zetva
                {
                    DatumIzvrsenja = dto.DatumIzvrsenja,
                    TipRadnje = dto.TipRadnje,
                    IdParcela = dto.IdParcela,
                    IdKultura = dto.IdKultura,
                    Napomena = dto.Napomena,
                    UkupanTrosak = dto.UkupanTrosak,
                    Prinos = (double)dto.Prinos!
                }
                : new Radnja
                {
                    DatumIzvrsenja = dto.DatumIzvrsenja,
                    TipRadnje = dto.TipRadnje,
                    IdParcela = dto.IdParcela,
                    IdKultura = dto.IdKultura,
                    Napomena = dto.Napomena,
                    UkupanTrosak = dto.UkupanTrosak
                };

            //  2. Snimi radnju
            var entity = await _radnjaRepository.Add(novaRadnja);

            //  3. Ako je SETVA → kreiraj ParcelaKultura sa IdSetvaRadnja
            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                var pkDto = new ParcelaKulturaDTO
                {
                    IdParcela = dto.IdParcela,
                    IdKultura = dto.IdKultura,
                    Povrsina = dto.Povrsina ?? 0,
                    DatumSetve = dto.DatumIzvrsenja,
                    IdSetvaRadnja = entity.Id
                };
                await _parcelaKulturaService.Add(pkDto);
            }
            // 4. Ako je ŽETVA → dopuni sve nezavršene setve za tu parcelu i kulturu
            else if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                var aktivneSetve = await _parcelaKulturaService.GetSveNezavrseneSetve(
                    dto.IdParcela.Value, dto.IdKultura.Value);

                if (aktivneSetve == null || !aktivneSetve.Any())
                    throw new Exception("Nema aktivnih setvi za ovu kulturu na parceli.");

                foreach (var setva in aktivneSetve)
                {
                    setva.DatumZetve = dto.DatumIzvrsenja;
                    setva.IdZetvaRadnja = entity.Id;
                    await _parcelaKulturaService.Update(setva);
                }
            }


            return entity.ToRadnjaDTO();
        }



        public async Task<RadnjaDTO> Update(Guid id, RadnjaDTO dto)
        {
            var staraRadnja = await _radnjaRepository.GetById(id);
            // GLOBALNA PROVERA: ako je setva i ima žetvu -> zabrani izmenu
            if (staraRadnja.TipRadnje == RadnjaTip.Setva)
            {
                var pk = await _parcelaKulturaService.GetByParcelaAndKulturaId(
                    staraRadnja.IdParcela!.Value, staraRadnja.IdKultura!.Value);

                if (pk != null && pk.IdSetvaRadnja == staraRadnja.Id && pk.IdZetvaRadnja != null)
                    throw new Exception("Setva ne može biti izmenjena jer je za nju već obavljena žetva.");
            }


            if (staraRadnja == null)
                throw new Exception("Radnja ne postoji.");

            if (staraRadnja.TipRadnje != dto.TipRadnje)
                throw new InvalidOperationException("Tip radnje se ne može menjati.");

            //  Ako je SETVA
            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                var pkList = await _parcelaKulturaService.GetAllByParcelaId(dto.IdParcela.Value);
                var target = pkList.FirstOrDefault(x => x.IdSetvaRadnja == id);

                // Ako setva ima žetvu — zabrani izmenu (fallback za svaki slučaj)
                if (target != null && target.IdZetvaRadnja != null)
                    throw new Exception("Setva ne može biti izmenjena jer je već obavljena žetva.");

                var parcela = await _radnjaRepository.GetParcelaSaSetvama(dto.IdParcela.Value);
                if (parcela == null)
                    throw new Exception("Parcela nije pronađena.");

                // Izračunaj slobodnu površinu (isključujući trenutnu setvu)
                var zauzeto = parcela.ParceleKulture
                    .Where(pk => pk.IdZetvaRadnja == null && pk.IdSetvaRadnja != id)
                    .Sum(pk => pk.Povrsina);

                var slobodno = parcela.Povrsina - zauzeto;
                if (dto.Povrsina > slobodno)
                    throw new Exception($"Nema dovoljno slobodne površine. Dostupno: {slobodno:F2} ha.");

                // Ažuriraj kulturu i površinu u povezanoj vezi
                if (target != null)
                {
                    bool promenjenaKultura = target.IdKultura != dto.IdKultura;

                    target.IdKultura = dto.IdKultura;
                    target.Povrsina = dto.Povrsina ?? target.Povrsina;
                    target.DatumSetve = dto.DatumIzvrsenja;

                    await _parcelaKulturaService.Update(target);

                    // ako je promenjena kultura, osveži i radnju
                    if (promenjenaKultura)
                    {
                        staraRadnja.IdKultura = dto.IdKultura;
                        await _radnjaRepository.Update(staraRadnja);
                    }
                }
            }


            //  Ako je ŽETVA
            if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                var pk = await _parcelaKulturaService.GetAllByParcelaId(dto.IdParcela.Value);
                var target = pk.FirstOrDefault(x => x.IdZetvaRadnja == id);

                if (target != null)
                {
                    target.DatumZetve = dto.DatumIzvrsenja;
                    await _parcelaKulturaService.Update(target);
                }
            }

            //  Na kraju ažuriraj samu radnju
            await _radnjaRepository.Update(dto.ToRadnja());
            return dto;
        }
        public async Task<bool> DeleteById(Guid id)
        {
            var radnja = await _radnjaRepository.GetById(id);
            if (radnja == null) return false;

            // 🟢 Ako je SETVA
            if (radnja.TipRadnje == RadnjaTip.Setva)
            {
                // Pronađi PK koja koristi ovu setvu
                var pk = await _parcelaKulturaService.GetBySetvaRadnjaId(id);

                if (pk != null)
                {
                    // Ako postoji žetva vezana za ovu setvu
                    if (pk.IdZetvaRadnja != null)
                    {
                        var zetvaId = pk.IdZetvaRadnja.Value;

                        // 1️⃣ Ukloni reference žetve iz svih PK koje koriste istu žetvu
                        var sveZaZetvu = await _parcelaKulturaService.GetSveZaZetvu(zetvaId);
                        foreach (var p in sveZaZetvu)
                        {
                            p.IdZetvaRadnja = null;
                            p.DatumZetve = null;
                            await _parcelaKulturaService.Update(p);
                        }

                        // 2️⃣ Obriši samu žetvu
                        var zetvaRadnja = await _radnjaRepository.GetById(zetvaId);
                        if (zetvaRadnja != null)
                            await _radnjaRepository.Delete(zetvaRadnja);
                    }

                    // 3️⃣ Obriši PK za ovu setvu
                    await _parcelaKulturaService.DeleteIfNotCompleted(
                        pk.IdParcela!.Value, pk.IdKultura!.Value, pk.IdSetvaRadnja!.Value);
                }
            }

            // Ako je ŽETVA
            else if (radnja.TipRadnje == RadnjaTip.Zetva)
            {
                // Očisti sve PK koje koriste ovu žetvu
                var pkList = await _parcelaKulturaService.GetSveZaZetvu(id);
                foreach (var pk in pkList)
                {
                    pk.IdZetvaRadnja = null;
                    pk.DatumZetve = null;
                    await _parcelaKulturaService.Update(pk);
                }
            }

            // 🧹 Na kraju obriši samu radnju
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

        public async Task UpdateUkupanTrosak(Guid idRadnja)
        {
            await _radnjaRepository.UpdateUkupanTrosak(idRadnja);
        }

        public async Task<decimal> GetSlobodnaPovrsinaAsync(Guid idParcela)
        {
            var parcela = await _radnjaRepository.GetParcelaSaSetvama(idParcela);
            if (parcela == null)
                throw new Exception("Parcela nije pronađena.");

            decimal zauzeto = parcela.ParceleKulture
                .Where(pk => pk.DatumZetve == null)
                .Sum(pk => pk.Povrsina);

            decimal slobodno = parcela.Povrsina - zauzeto;
            return slobodno < 0 ? 0 : slobodno;
        }

    }
}
