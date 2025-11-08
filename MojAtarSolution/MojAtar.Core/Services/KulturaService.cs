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
    public class KulturaService : IKulturaService
    {
        private readonly IKulturaRepository _kulturaRepository;
        private readonly IRadnjaRepository _radnjaRepository;
        private readonly ICenaKultureService _cenaKultureService;

        public KulturaService(
            IKulturaRepository kulturaRepository,
            IRadnjaRepository radnjaRepository,
            ICenaKultureService cenaKultureService)
        {
            _kulturaRepository = kulturaRepository;
            _radnjaRepository = radnjaRepository;
            _cenaKultureService = cenaKultureService;
        }


        public async Task<KulturaDTO> Add(KulturaDTO kulturaAdd)
        {
            if (kulturaAdd == null)
            {
                throw new ArgumentNullException(nameof(kulturaAdd));
            }

            if (kulturaAdd.Naziv == null)
            {
                throw new ArgumentException(nameof(kulturaAdd.Naziv));
            }

            var existing = await _kulturaRepository.GetByNazivIKorisnik(kulturaAdd.Naziv, kulturaAdd.IdKorisnik);
            if (existing != null)
            {
                throw new ArgumentException("Već postoji entitet sa ovim nazivom za vaš nalog.");
            }


            Kultura kultura = kulturaAdd.ToKultura();

            kultura.Id = Guid.NewGuid();

            await _kulturaRepository.Add(kultura);

            CenaKulture cena = new CenaKulture
            {
                Id = Guid.NewGuid(),
                IdKultura = kultura.Id.Value,
                CenaPojedinici = kultura.AktuelnaCena,
                DatumVaznosti = kulturaAdd.DatumVaznostiCene != DateTime.MinValue ? kulturaAdd.DatumVaznostiCene : DateTime.Now
            };
            await _kulturaRepository.DodajCenu(cena); 


            return kultura.ToKulturaDTO();
        }

        public async Task<bool> DeleteById(Guid? id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var kultura = await _kulturaRepository.GetById(id.Value);
            if (kultura == null)
                return false;

            // 🔹 Nađi sve radnje za ovu kulturu
            var radnjeZaKulturu = await _radnjaRepository.GetAllByKultura(id.Value);
            foreach (var radnja in radnjeZaKulturu)
            {
                if (radnja.TipRadnje == RadnjaTip.Setva || radnja.TipRadnje == RadnjaTip.Zetva)
                {
                    await _radnjaRepository.Delete(radnja);
                }
            }


            // 🔹 Obriši samu kulturu
            await _kulturaRepository.DeleteKulturaById(id.Value);
            return true;
        }


        public async Task<List<KulturaDTO>> GetAllForUser(Guid idKorisnika)
        {
            var kulture = await _kulturaRepository.GetAllByKorisnik(idKorisnika);
            var danas = DateTime.Now;

            var result = new List<KulturaDTO>();
            foreach (var k in kulture)
            {
                double aktuelnaCena = await _cenaKultureService.GetAktuelnaCena(idKorisnika, k.Id.Value, danas);
                var dto = k.ToKulturaDTO();
                dto.AktuelnaCena = aktuelnaCena;
                result.Add(dto);
            }
            return result;
        }

        public async Task<List<KulturaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            var kulture = await _kulturaRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);
            var danas = DateTime.Now;

            var result = new List<KulturaDTO>();
            foreach (var k in kulture)
            {
                double aktuelnaCena = await _cenaKultureService.GetAktuelnaCena(idKorisnik, k.Id.Value, danas);
                var dto = k.ToKulturaDTO();
                dto.AktuelnaCena = aktuelnaCena;
                result.Add(dto);
            }
            return result;
        }

        public async Task<KulturaDTO> GetById(Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Kultura? kultura = await _kulturaRepository.GetById(id.Value);

            if (kultura == null) return null;

            return kultura.ToKulturaDTO();
        }
        public async Task<KulturaDTO> GetByNaziv(string? naziv, Guid idKorisnik)
        {
            if (naziv == null)
            {
                throw new ArgumentNullException(nameof(naziv));
            }

            Kultura? kultura = await _kulturaRepository.GetByNazivIKorisnik(naziv, idKorisnik);

            if (kultura == null) return null;

            return kultura.ToKulturaDTO();
        }

        public async Task<KulturaDTO> Update(Guid? id, KulturaDTO dto)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var staraKultura = await _kulturaRepository.GetById(id.Value);
            if (staraKultura == null)
                return null;

            // 🔹 Provera da li već postoji druga kultura sa istim nazivom
            var postoji = await _kulturaRepository.GetByNazivIKorisnik(dto.Naziv, dto.IdKorisnik);
            if (postoji != null && postoji.Id != id)
                throw new ArgumentException("Već postoji kultura sa ovim nazivom za vaš nalog.");

            // 🔹 Provera da li se cena promenila
            if (staraKultura.AktuelnaCena != dto.AktuelnaCena)
            {
                // Kreiramo novi zapis u istoriji cena
                var novaCena = new CenaKulture
                {
                    Id = Guid.NewGuid(),
                    IdKultura = id.Value,
                    CenaPojedinici = (double)dto.AktuelnaCena,
                    DatumVaznosti = dto.DatumVaznostiCene != DateTime.MinValue
                                    ? dto.DatumVaznostiCene
                                    : DateTime.Now
                };

                await _kulturaRepository.DodajCenu(novaCena);

                // Ako je nova cena aktuelna (datum danas ili budućnost) → ažuriraj i glavnu tabelu
                if (novaCena.DatumVaznosti >= DateTime.Now.Date)
                {
                    staraKultura.AktuelnaCena = (double)dto.AktuelnaCena;
                }
                // Ako je unet stariji datum → samo istorijski zapis, bez promene aktuelne cene
            }

            // Ažuriranje osnovnih polja kulture
            staraKultura.Naziv = dto.Naziv;
            staraKultura.IdKorisnik = dto.IdKorisnik;

            await _kulturaRepository.Update(staraKultura);
            return staraKultura.ToKulturaDTO();
        }

        public async Task<int> GetCountByKorisnik(Guid idKorisnik)
        {
            return await _kulturaRepository.GetCountByKorisnik(idKorisnik);
        }
        public async Task<KulturaDTO> GetWithAktuelnaCena(Guid idKorisnik, Guid idKultura)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null) return null;

            var aktuelnaCena = await _cenaKultureService.GetAktuelnaCena(idKorisnik, idKultura, DateTime.Now);
            var datumCene = await _cenaKultureService.GetDatumAktuelneCene(idKultura, DateTime.Now);

            var dto = kultura.ToKulturaDTO();
            dto.AktuelnaCena = aktuelnaCena;
            dto.DatumVaznostiCene = datumCene ?? DateTime.Now;

            return dto;
        }
        public async Task AzurirajPosleZetve(Guid idKultura, decimal dodatiPrinos)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null)
                throw new Exception("Kultura nije pronađena.");

            kultura.RaspolozivoZaProdaju += dodatiPrinos;
            await _kulturaRepository.Update(kultura);
        }

        public async Task AzurirajPosleProdaje(Guid idKultura, decimal kolicina)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null)
                throw new Exception("Kultura nije pronađena.");

            if (kultura.RaspolozivoZaProdaju < kolicina)
                throw new Exception($"Nema dovoljno raspoložive količine ({kultura.RaspolozivoZaProdaju} kg).");

            kultura.RaspolozivoZaProdaju -= kolicina;
            await _kulturaRepository.Update(kultura);
        }

        public async Task VratiPosleBrisanjaProdaje(Guid idKultura, decimal kolicina)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null)
                return;

            kultura.RaspolozivoZaProdaju += kolicina;
            await _kulturaRepository.Update(kultura);
        }

        public async Task AzurirajPosleIzmeneZetve(Guid idKultura, decimal stariPrinos, decimal noviPrinos)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null)
                throw new Exception("Kultura nije pronađena.");

            var razlika = noviPrinos - stariPrinos;

            if (razlika < 0 && kultura.RaspolozivoZaProdaju + razlika < 0)
                throw new Exception("Smanjenjem prinosa raspoloživo bi palo u negativno.");

            kultura.RaspolozivoZaProdaju += razlika;
            await _kulturaRepository.Update(kultura);
        }

        public async Task<bool> MozeSmanjenje(Guid idKultura, decimal razlika)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null)
                return false;

            return kultura.RaspolozivoZaProdaju - razlika >= 0;
        }

        public async Task<bool> MozeBrisanjeZetve(Guid idKultura, decimal prinos)
        {
            var kultura = await _kulturaRepository.GetById(idKultura);
            if (kultura == null)
                return false;

            return kultura.RaspolozivoZaProdaju - prinos >= 0;
        }
    }
}
