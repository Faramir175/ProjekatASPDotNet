using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.Extensions;
using MojAtar.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MojAtar.Core.Services
{
    public class ProdajaService : IProdajaService
    {
        private readonly IProdajaRepository _prodajaRepository;
        private readonly ICenaKultureService _cenaKultureService;

        public ProdajaService(IProdajaRepository prodajaRepository, ICenaKultureService cenaKultureService)
        {
            _prodajaRepository = prodajaRepository;
            _cenaKultureService = cenaKultureService;
        }

        public async Task<List<ProdajaDTO>> GetAllByKorisnik(Guid korisnikId)
        {
            var prodaje = await _prodajaRepository.GetAllByKorisnik(korisnikId);
            return prodaje.Select(p => p.ToProdajaDTO()!).ToList();
        }

        public async Task Add(ProdajaDTO dto)
        {
            if (dto.Kolicina == null)
                throw new InvalidOperationException("Morate uneti količinu.");

            // proveri raspoloživu količinu
            decimal ukupnoProizvedeno = await _prodajaRepository.GetUkupanPrinosZaKulturu(dto.IdKultura);
            decimal ukupnoProdato = await _prodajaRepository.GetUkupnoProdatoZaKulturu(dto.IdKultura);
            decimal raspolozivo = ukupnoProizvedeno - ukupnoProdato;

            if (dto.Kolicina.Value > raspolozivo)
                throw new InvalidOperationException($"Nema dovoljno raspoložive količine ({raspolozivo} kg).");

            var kultura = await _prodajaRepository.GetKulturaById(dto.IdKultura);
            if (kultura == null)
                throw new KeyNotFoundException("Kultura nije pronađena.");

            double aktuelnaCena = await _cenaKultureService.GetAktuelnaCena(
                kultura.IdKorisnik, kultura.Id!.Value, dto.DatumProdaje);

            dto.CenaPoJedinici = (decimal)aktuelnaCena;

            var entity = dto.ToProdaja();
            await _prodajaRepository.Add(entity);
        }

        public async Task Update(ProdajaDTO dto)
        {
            if (dto.Kolicina == null)
                throw new InvalidOperationException("Morate uneti količinu.");

            var stara = await _prodajaRepository.GetById(dto.Id!.Value);
            if (stara == null)
                throw new KeyNotFoundException("Prodaja nije pronađena.");

            decimal ukupnoProizvedeno = await _prodajaRepository.GetUkupanPrinosZaKulturu(dto.IdKultura);
            decimal ukupnoProdatoBezOve = await _prodajaRepository.GetUkupnoProdatoZaKulturu(dto.IdKultura) - stara.Kolicina;
            decimal raspolozivo = ukupnoProizvedeno - ukupnoProdatoBezOve;

            if (dto.Kolicina.Value > raspolozivo)
                throw new InvalidOperationException($"Nema dovoljno raspoložive količine ({raspolozivo} kg).");

            stara.Kolicina = dto.Kolicina.Value;
            stara.CenaPoJedinici = dto.CenaPoJedinici ?? stara.CenaPoJedinici;
            stara.DatumProdaje = dto.DatumProdaje;
            stara.Napomena = dto.Napomena;

            await _prodajaRepository.Update(stara);
        }


        public async Task Delete(Guid id)
        {
            await _prodajaRepository.Delete(id);
        }

        public async Task<ProdajaDTO?> GetById(Guid id)
        {
            var entity = await _prodajaRepository.GetById(id);
            return entity?.ToProdajaDTO();
        }
        public async Task<decimal> GetUkupanPrinosZaKulturu(Guid idKultura)
        {
            return await _prodajaRepository.GetUkupanPrinosZaKulturu(idKultura);
        }

        public async Task<decimal> GetUkupnoProdatoZaKulturu(Guid idKultura)
        {
            return await _prodajaRepository.GetUkupnoProdatoZaKulturu(idKultura);
        }

        public async Task<List<ProdajaDTO>> GetPaged(Guid korisnikId, int skip, int take)
        {
            var prodaje = await _prodajaRepository.GetPaged(korisnikId, skip, take);
            return prodaje.Select(p => p.ToProdajaDTO()!).ToList();
        }

        public async Task<int> GetTotalCount(Guid korisnikId)
        {
            return await _prodajaRepository.GetTotalCount(korisnikId);
        }
        public async Task<IzvestajProdajeResult?> GetIzvestajProdaje(Guid korisnikId, DateTime? odDatuma, DateTime? doDatuma)
        {
            var prodaje = await _prodajaRepository.GetByKorisnikAndPeriod(korisnikId, odDatuma, doDatuma);
            if (!prodaje.Any())
                return null;

            var radnjeProdaje = prodaje.Select(p => new RadnjaIzvestajDTO
            {
                Id = p.Id,
                NazivRadnje = "Prodaja",
                Datum = p.DatumProdaje,
                Kultura = p.Kultura?.Naziv ?? "(nepoznata kultura)",
                IdKultura = p.IdKultura,
                Prinos = (decimal)p.Kolicina,
                Prihod = (decimal)(p.Kolicina * p.CenaPoJedinici),
                Trosak = 0,
            }).OrderByDescending(r => r.Datum).ToList();

            var prinosi = await _prodajaRepository.GetPrinosPoKulturi(korisnikId, odDatuma, doDatuma);

            // Zbir po kulturi
            var zbirPoKulturi = radnjeProdaje
                .GroupBy(r => new { r.IdKultura, r.Kultura })
                .Select(g =>
                {
                    prinosi.TryGetValue(g.Key.IdKultura ?? Guid.Empty, out decimal ukupanPrinos);

                    return new RadnjaIzvestajDTO
                    {
                        Id = Guid.NewGuid(),
                        NazivRadnje = "Ukupno po kulturi",
                        Datum = DateTime.Now,
                        Kultura = g.Key.Kultura,
                        IdKultura = g.Key.IdKultura,
                        Prinos = g.Sum(x => x.Prinos), // prodato
                        UkupanPrinosKulture = ukupanPrinos,
                        Prihod = g.Sum(x => x.Prihod),
                        Trosak = 0
                    };
                })
                .ToList();

            decimal ukupniPrihod = zbirPoKulturi.Sum(x => x.Prihod);

            var prodajeParcela = new ParcelaIzvestajDTO
            {
                Id = Guid.NewGuid(),
                NazivParcele = "📦 Ukupne prodaje po kulturama",
                Radnje = radnjeProdaje.Concat(zbirPoKulturi).ToList()
            };

            return new IzvestajProdajeResult
            {
                ParcelaProdaje = prodajeParcela,
                UkupanPrihod = ukupniPrihod
            };
        }


    }
}
