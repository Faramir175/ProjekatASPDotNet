using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Domain.RepositoryContracts;

namespace MojAtar.Core.Services
{
    public class IzvestajService : IIzvestajService
    {
        private readonly IIzvestajRepository _izvestajRepository;
        private readonly ICenaResursaService _cenaResursaService;
        private readonly IProdajaService _prodajaService;

        public IzvestajService(
            IIzvestajRepository izvestajRepository,
            ICenaResursaService cenaResursaService,
            IProdajaService prodajaService)
        {
            _izvestajRepository = izvestajRepository;
            _cenaResursaService = cenaResursaService;
            _prodajaService = prodajaService;
        }

        public async Task<IzvestajDTO> GenerisiIzvestaj(
            Guid userId, Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele)
        {
            // 1. Poziv repozitorijuma (vraća već filtrirane podatke)
            var parcele = await _izvestajRepository.GetIzvestaj(userId, odDatuma, doDatuma, parcelaId, sveParcele);

            // 2. Poslovna logika: Uklanjamo prazne parcele (ako postoje)
            parcele = parcele.Where(p => p.Radnje.Any()).ToList();

            // 3. Obogaćivanje podacima (Cene resursa)
            foreach (var parcela in parcele)
            {
                foreach (var radnja in parcela.Radnje)
                {
                    foreach (var resurs in radnja.Resursi)
                    {
                        if (resurs.IdResurs != Guid.Empty)
                        {
                            decimal cenaResursa = (decimal)await _cenaResursaService.GetAktuelnaCena(
                                userId, resurs.IdResurs, resurs.DatumKoriscenja);

                            resurs.JedinicnaCena = (double)cenaResursa;
                        }
                    }
                    radnja.Trosak = (decimal)radnja.Resursi.Sum(r => r.Kolicina * r.JedinicnaCena);
                    radnja.Prihod = 0;
                }
            }

            // 4. Prodaja (Samo ako su izabrane sve parcele)
            decimal ukupanPrihodIzProdaja = 0;
            if (sveParcele)
            {
                var prodajeIzvestaj = await _prodajaService.GetIzvestajProdaje(userId, odDatuma, doDatuma);
                if (prodajeIzvestaj != null)
                {
                    parcele.Add(prodajeIzvestaj.ParcelaProdaje);
                    ukupanPrihodIzProdaja = prodajeIzvestaj.UkupanPrihod;
                }
            }

            // 5. Validacija: Da li ima podataka?
            if (!parcele.Any())
            {
                // Ovo će kontroler uhvatiti i prikazati poruku
                throw new ArgumentException("Nema dostupnih podataka za prikaz u izabranom periodu.");
            }

            // 6. Izračunavanje ukupnih totala (BITNO za prikaz na View-u)
            decimal ukupanTrosak = parcele
                .Where(p => !p.NazivParcele.Contains("prodaje")) // Isključujemo prodaju iz troškova
                .Sum(p => p.Radnje.Sum(r => r.Trosak));

            decimal ukupanPrihod = ukupanPrihodIzProdaja;
            decimal profit = ukupanPrihod - ukupanTrosak;

            // 7. Vraćanje kompletnog DTO-a
            return new IzvestajDTO
            {
                DatumOd = odDatuma ?? DateTime.MinValue,
                DatumDo = doDatuma ?? DateTime.MaxValue,
                Parcele = parcele,
                UkupanPrihodIzProdaja = ukupanPrihodIzProdaja,

            };
        }
    }
}