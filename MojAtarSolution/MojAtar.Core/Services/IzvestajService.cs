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

        public IzvestajService(IIzvestajRepository izvestajRepository, ICenaKultureService cenaKultureService, ICenaResursaService cenaResursaService)
        {
            _izvestajRepository = izvestajRepository;
            _cenaKultureService = cenaKultureService;
            _cenaResursaService = cenaResursaService;
        }

        public async Task<IzvestajDTO> GenerisiIzvestaj(
                    Guid userId, Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele)
        {
            var parcele = await _izvestajRepository.GetIzvestaj(userId, odDatuma, doDatuma, parcelaId, sveParcele);

            foreach (var parcela in parcele)
            {
                foreach (var radnja in parcela.Radnje)
                {
                    // --- 1. Dohvatanje cene kulture
                    if (radnja.IdKultura != Guid.Empty)
                    {
                        double cenaKulture = await _cenaKultureService.GetAktuelnaCena(
                            userId, radnja.IdKultura.Value, radnja.Datum);

                        if (radnja.NazivRadnje == "Zetva")
                        {
                            radnja.Prihod = radnja.Prinos * cenaKulture;
                        }
                    }


                    // --- 2. Dohvatanje cene resursa
                    foreach (var resurs in radnja.Resursi)
                    {
                        if (resurs.IdResurs != Guid.Empty)
                        {
                            double cenaResursa = await _cenaResursaService.GetAktuelnaCena(
                                userId, resurs.IdResurs, resurs.DatumKoriscenja);

                            resurs.JedinicnaCena = cenaResursa;
                        }

                    }
                }
            }

            return new IzvestajDTO
            {
                DatumOd = odDatuma ?? DateTime.MinValue,
                DatumDo = doDatuma ?? DateTime.MaxValue,
                Parcele = parcele
            };
        }
    }
}
