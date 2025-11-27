using MojAtar.Core.Domain.Enums;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.DTO.Extensions;
using MojAtar.Core.ServiceContracts;

namespace MojAtar.Core.Services
{
    public class PocetnaService : IPocetnaService
    {
        private readonly IParcelaRepository _parcelaRepo;
        private readonly IRadnjaRepository _radnjaRepo;
        private readonly IResursRepository _resursRepo;
        private readonly IRadnaMasinaRepository _radnaRepo;
        private readonly IPrikljucnaMasinaRepository _prikljucnaRepo;
        private readonly IKulturaRepository _kulturaRepo;
        private readonly IParcelaKulturaRepository _parcelaKulturaRepo;

        public PocetnaService(
            IParcelaRepository parcelaRepo,
            IRadnjaRepository radnjaRepo,
            IResursRepository resursRepo,
            IRadnaMasinaRepository radnaRepo,
            IPrikljucnaMasinaRepository prikljucnaRepo,
            IKulturaRepository kulturaRepo,
            IParcelaKulturaRepository parcelaKulturaRepo)
        {
            _parcelaRepo = parcelaRepo;
            _radnjaRepo = radnjaRepo;
            _resursRepo = resursRepo;
            _radnaRepo = radnaRepo;
            _prikljucnaRepo = prikljucnaRepo;
            _kulturaRepo = kulturaRepo;
            _parcelaKulturaRepo = parcelaKulturaRepo;
        }

        public async Task<PocetnaDTO> GetDashboardDataAsync(Guid korisnikId)
        {
            // Ovo sprečava "Concurrency" grešku u DbContext-u

            var brojParcela = await _parcelaRepo.CountByKorisnikId(korisnikId);
            var brojRadnji = await _radnjaRepo.CountByKorisnikId(korisnikId);
            var brojResursa = await _resursRepo.CountByKorisnikId(korisnikId);
            var brojMasina = await _radnaRepo.CountByKorisnikId(korisnikId);
            var brojPrikljucnih = await _prikljucnaRepo.CountByKorisnikId(korisnikId);
            var brojKultura = await _kulturaRepo.CountByKorisnikId(korisnikId);
            var poslednjeRadnje = await _radnjaRepo.GetLastRadnjeByKorisnik(korisnikId, 3);

            var radnjaDTOs = new List<RadnjaDTO>();

            foreach (var r in poslednjeRadnje)
            {
                double? povrsinaZaSetvu = null;

                if (r.TipRadnje == RadnjaTip.Setva && r.Id.HasValue)
                {
                    // Ovo je ok jer se izvršava sekvencijalno unutar petlje
                    var parcelaKultura = await _parcelaKulturaRepo.GetBySetvaRadnjaId(r.Id.Value);
                    if (parcelaKultura != null)
                    {
                        povrsinaZaSetvu = (double?)parcelaKultura.Povrsina;
                    }
                }

                radnjaDTOs.Add(r.ToRadnjaDTO(povrsina: (decimal?)povrsinaZaSetvu));
            }

            return new PocetnaDTO
            {
                BrojParcela = brojParcela,
                BrojRadnji = brojRadnji,
                BrojResursa = brojResursa,
                BrojRadnihMasina = brojMasina,
                BrojPrikljucnihMasina = brojPrikljucnih,
                BrojKultura = brojKultura,
                PoslednjeRadnje = radnjaDTOs
            };
        }
    }
}