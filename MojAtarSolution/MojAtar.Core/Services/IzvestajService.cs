using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Domain.RepositoryContracts;

namespace MojAtar.Core.Services
{
    public class IzvestajService : IIzvestajService
    {
        private readonly IIzvestajRepository _izvestajRepository;
        private readonly ICenaKultureService _cenaKultureService;
        private readonly ICenaResursaService _cenaResursaService;
        private readonly IProdajaService _prodajaService;

        public IzvestajService(
            IIzvestajRepository izvestajRepository,
            ICenaKultureService cenaKultureService,
            ICenaResursaService cenaResursaService,
            IProdajaService prodajaService)
        {
            _izvestajRepository = izvestajRepository;
            _cenaKultureService = cenaKultureService;
            _cenaResursaService = cenaResursaService;
            _prodajaService = prodajaService;
        }

        public async Task<IzvestajDTO> GenerisiIzvestaj(
    Guid userId, Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele)
        {
            var parcele = await _izvestajRepository.GetIzvestaj(userId, odDatuma, doDatuma, parcelaId, sveParcele);

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
                    radnja.Prihod = 0; // prihod se ne računa iz radnji
                }
            }

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

            return new IzvestajDTO
            {
                DatumOd = odDatuma ?? DateTime.MinValue,
                DatumDo = doDatuma ?? DateTime.MaxValue,
                Parcele = parcele,
                UkupanPrihodIzProdaja = ukupanPrihodIzProdaja
            };
        }

    }
}