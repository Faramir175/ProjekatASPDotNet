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
            var staraRadnja = await _radnjaRepository.GetById(id);
            if (staraRadnja == null)
                throw new ArgumentException("Radnja ne postoji.");

            // Ako se tip radnje promenio, možemo baciti grešku jer to ne bi trebalo da se dešava
            if (staraRadnja.TipRadnje != dto.TipRadnje)
                throw new InvalidOperationException("Tip radnje se ne može menjati.");

            // Ako je obična radnja
            if (dto.TipRadnje != RadnjaTip.Setva && dto.TipRadnje != RadnjaTip.Zetva)
            {
                await _radnjaRepository.Update(dto.ToRadnja());
                return dto;
            }

            // --- SETVA ---
            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                var parcela = await _radnjaRepository.GetParcelaSaSetvama((Guid)dto.IdParcela);
                if (parcela == null)
                    throw new Exception("Parcela nije pronađena.");

                var nezavrseneSetve = parcela.ParceleKulture
                    .Where(pk => pk.DatumZetve == null)
                    .ToList();

                // Isključi trenutnu setvu koja se menja, da ne računa samu sebe dvaput
                var setveZaRacunanje = nezavrseneSetve
                    .Where(pk => !(pk.IdKultura == dto.IdKultura && pk.DatumSetve == staraRadnja.DatumIzvrsenja))
                    .ToList();

                decimal slobodno = parcela.Povrsina - setveZaRacunanje.Sum(pk => pk.Povrsina);

                if (dto.Povrsina > slobodno)
                {
                    throw new Exception($"Nema dovoljno slobodne površine. Dostupno: {slobodno:F2} ha.");
                }
                // Ako se promenila kultura
                if (staraRadnja.IdKultura != dto.IdKultura)
                {
                    // proveri da li postoji zavrsena setva (ima DatumZetve)
                    var staraPK = await _parcelaKulturaService
                        .GetByParcelaAndKulturaId(staraRadnja.IdParcela.Value, staraRadnja.IdKultura.Value);

                    if (staraPK != null && staraPK.DatumZetve != null)
                    {
                        throw new InvalidOperationException(
                            "Nije dozvoljeno menjati kulturu setve za koju je već unešena žetva."
                        );
                    }

                    // Ako nije završena, briši staru i dodaj novu
                    var nezavrsena = await _parcelaKulturaService.GetNezavrsenaSetva(
                        staraRadnja.IdParcela.Value, staraRadnja.IdKultura.Value);

                    if (nezavrsena != null)
                        await _parcelaKulturaService.Delete(nezavrsena.Id.Value);

                    await _parcelaKulturaService.Add(new ParcelaKulturaDTO
                    {
                        IdParcela = dto.IdParcela,
                        IdKultura = dto.IdKultura,
                        Povrsina = (decimal)dto.Povrsina,
                        DatumSetve = dto.DatumIzvrsenja,
                    });
                }
                else
                {
                    var postojeca = await _parcelaKulturaService
                        .GetNezavrsenaSetva(dto.IdParcela.Value, dto.IdKultura.Value);

                    if (postojeca != null && postojeca.Povrsina != dto.Povrsina)
                    {
                        await _parcelaKulturaService.UpdateNezavrsena(
                            dto.IdParcela.Value, dto.IdKultura.Value, (decimal)dto.Povrsina
                        );
                    }
                }
            }

            // --- ŽETVA ---
            if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                if (staraRadnja.IdKultura != dto.IdKultura)
                {
                    // poništi stari datum žetve za staru kulturu
                    var staraSetva = await _parcelaKulturaService.GetByParcelaAndKulturaId(
                        staraRadnja.IdParcela.Value, staraRadnja.IdKultura.Value);

                    if (staraSetva != null)
                    {
                        staraSetva.DatumZetve = null;
                        await _parcelaKulturaService.Update(staraSetva);
                    }

                    // pronađi novu kulturu i postavi datum žetve
                    var novaSetva = await _parcelaKulturaService.GetNezavrsenaSetva(
                        dto.IdParcela.Value, dto.IdKultura.Value);

                    if (novaSetva == null)
                        throw new InvalidOperationException("Ne postoji aktivna setva za novu kulturu!");

                    novaSetva.DatumZetve = dto.DatumIzvrsenja;
                    await _parcelaKulturaService.Update(novaSetva);
                }
                else
                {
                    // ako kultura nije promenjena, samo osveži prinos i eventualno datum žetve
                    var setva = await _parcelaKulturaService.GetNezavrsenaSetva(
                        dto.IdParcela.Value, dto.IdKultura.Value);

                    if (setva != null)
                    {
                        setva.DatumZetve = dto.DatumIzvrsenja;
                        await _parcelaKulturaService.Update(setva);
                    }
                }
            }

            await _radnjaRepository.Update(dto.ToRadnja());
            return dto;
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
            if (radnja.TipRadnje == RadnjaTip.Zetva)
            {
                // poništava datum žetve ako je radnja obrisana
                var parcelaKultura = await _parcelaKulturaService
                    .GetByParcelaAndKulturaId((Guid)radnja.IdParcela, (Guid)radnja.IdKultura);

                if (parcelaKultura != null && parcelaKultura.DatumZetve != null)
                {
                    parcelaKultura.DatumZetve = null;
                    await _parcelaKulturaService.Update(parcelaKultura);
                }
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
