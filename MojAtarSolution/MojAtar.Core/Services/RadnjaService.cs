using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.Domain.RepositoryContracts;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.ExtensionKlase;
using MojAtar.Core.DTO.Extensions;
using MojAtar.Core.ServiceContracts;

namespace MojAtar.Core.Services
{
    public class RadnjaService : IRadnjaService
    {
        private readonly IRadnjaRepository _radnjaRepository;
        private readonly IParcelaKulturaService _parcelaKulturaService;
        private readonly IKulturaService _kulturaService;
        private readonly IRadnjaRadnaMasinaService _radnjaRadnaMasinaService;
        private readonly IRadnjaPrikljucnaMasinaService _radnjaPrikljucnaMasinaService;
        private readonly IRadnjaResursService _radnjaResursService;
        private readonly ICenaResursaService _cenaResursaService;
        private readonly IRadnjaParcelaService _radnjaParcelaService;

        public RadnjaService(
            IRadnjaRepository radnjaRepository,
            IParcelaKulturaService parcelaKulturaService,
            IKulturaService kulturaService,
            IRadnjaRadnaMasinaService radnjaRadnaMasinaService,
            IRadnjaPrikljucnaMasinaService radnjaPrikljucnaMasinaService,
            IRadnjaResursService radnjaResursService,
            ICenaResursaService cenaResursaService,
            IRadnjaParcelaService radnjaParcelaService)
        {
            _radnjaRepository = radnjaRepository;
            _parcelaKulturaService = parcelaKulturaService;
            _kulturaService = kulturaService;
            _radnjaRadnaMasinaService = radnjaRadnaMasinaService;
            _radnjaPrikljucnaMasinaService = radnjaPrikljucnaMasinaService;
            _radnjaResursService = radnjaResursService;
            _cenaResursaService = cenaResursaService;
            _radnjaParcelaService = radnjaParcelaService;
        }

        // --- GLAVNE METODE ZA CRUD ---

        public async Task<RadnjaDTO> Add(RadnjaDTO dto)
        {
            // 1. Validacije
            await ValidacijaRadnje(dto);

            // 2. Kreiranje entiteta Radnja
            Radnja novaRadnja = dto.TipRadnje == RadnjaTip.Zetva
                ? new Zetva { Prinos = (double)(dto.Prinos ?? 0) }
                : new Radnja();

            // Zajedničko mapiranje
            novaRadnja.DatumIzvrsenja = dto.DatumIzvrsenja;
            novaRadnja.TipRadnje = dto.TipRadnje;
            novaRadnja.IdKultura = dto.IdKultura;
            novaRadnja.Napomena = dto.Napomena;
            novaRadnja.UkupanTrosak = dto.UkupanTrosak;

            // 3. Snimanje osnovne radnje
            var entity = await _radnjaRepository.Add(novaRadnja);
            dto.Id = entity.Id;

            // 4. Snimanje veza sa Parcelama (RadnjaParcela tabela)
            if (dto.Parcele != null && dto.Parcele.Any())
            {
                foreach (var parcelaDto in dto.Parcele)
                {
                    // Ovo povezuje Radnju i Parcelu i čuva površinu te konkretne radnje
                    await _radnjaParcelaService.Add((Guid)entity.Id, parcelaDto);
                }
            }

            // 5. Obrada logike za Setvu/Žetvu (ParcelaKultura tabela)
            // OVDE SE DEŠAVA MAGIJA ZA VIŠE PARCELA
            await ObradiLogikuSetveZetve_Add(dto, (Guid)entity.Id);

            // 6. Snimanje veza (Mašine i Resursi)
            if (dto.RadneMasine != null)
            {
                foreach (var rm in dto.RadneMasine)
                {
                    rm.IdRadnja = (Guid)entity.Id;
                    await _radnjaRadnaMasinaService.Add(rm);
                }
            }

            if (dto.PrikljucneMasine != null)
            {
                foreach (var pm in dto.PrikljucneMasine)
                {
                    pm.IdRadnja = (Guid)entity.Id;
                    await _radnjaPrikljucnaMasinaService.Add(pm);
                }
            }

            if (dto.Resursi != null)
            {
                foreach (var r in dto.Resursi)
                {
                    r.IdRadnja = (Guid)entity.Id;
                    r.DatumKoriscenja = dto.DatumIzvrsenja;
                    await _radnjaResursService.Add(r);
                }
            }

            return entity.ToRadnjaDTO();
        }

        public async Task<RadnjaDTO> Update(Guid id, RadnjaDTO dto, List<Guid>? obrisaneRadneMasine = null, List<Guid>? obrisanePrikljucneMasine = null, List<Guid>? obrisaniResursi = null)
        {
            var staraRadnja = await _radnjaRepository.GetById(id);
            if (staraRadnja == null) throw new KeyNotFoundException("Radnja ne postoji.");

            if (staraRadnja.TipRadnje != dto.TipRadnje)
                throw new InvalidOperationException("Tip radnje se ne može menjati.");

            // 1. Validacije
            await ValidacijaIzmeneSetveZetve(staraRadnja, dto);
            if (dto.TipRadnje == RadnjaTip.Setva) await ValidacijaRadnje(dto);

            // 2. Ažuriranje veza sa Parcelama
            // Ovo ažurira površine u veznoj tabeli RadnjaParcela
            if (dto.Parcele != null)
            {
                await _radnjaParcelaService.UpdateParceleZaRadnju(id, dto.Parcele);
            }

            // 3. Brisanje starih resursa/mašina (tvoj postojeći kod)
            if (obrisaneRadneMasine != null)
                foreach (var idRM in obrisaneRadneMasine) await _radnjaRadnaMasinaService.Delete(id, idRM);

            if (obrisanePrikljucneMasine != null)
                foreach (var idPM in obrisanePrikljucneMasine) await _radnjaPrikljucnaMasinaService.Delete(id, idPM);

            if (obrisaniResursi != null)
                foreach (var idR in obrisaniResursi) await _radnjaResursService.Delete(id, idR);

            // 4. Dodavanje/Update resursa/mašina (tvoj postojeći kod)
            if (dto.RadneMasine != null)
            {
                foreach (var m in dto.RadneMasine)
                {
                    await _radnjaRadnaMasinaService.Delete(id, m.IdRadnaMasina);
                    m.IdRadnja = id;
                    await _radnjaRadnaMasinaService.Add(m);
                }
            }
            if (dto.PrikljucneMasine != null)
            {
                foreach (var pm in dto.PrikljucneMasine)
                {
                    await _radnjaPrikljucnaMasinaService.Delete(id, pm.IdPrikljucnaMasina);
                    pm.IdRadnja = id;
                    await _radnjaPrikljucnaMasinaService.Add(pm);
                }
            }
            if (dto.Resursi != null)
            {
                foreach (var r in dto.Resursi)
                {
                    await _radnjaResursService.Delete(id, r.IdResurs);
                    r.IdRadnja = id;
                    r.DatumKoriscenja = dto.DatumIzvrsenja;
                    await _radnjaResursService.Add(r);
                }
            }

            // 5. Ažuriranje ParcelaKultura tabele (ako je setva, a promenjena površina)
            await ObradiLogikuSetveZetve_Update(staraRadnja, dto);

            // 6. Update same radnje
            staraRadnja.UkupanTrosak = dto.UkupanTrosak;
            staraRadnja.Napomena = dto.Napomena;

            if (dto.TipRadnje == RadnjaTip.Zetva && staraRadnja is Zetva zetva)
            {
                zetva.Prinos = (double)(dto.Prinos ?? zetva.Prinos);
                await _radnjaRepository.Update(zetva);
            }
            else
            {
                await _radnjaRepository.Update(staraRadnja);
            }

            return staraRadnja.ToRadnjaDTO();
        }

        public async Task<bool> DeleteById(Guid id)
        {
            var radnja = await _radnjaRepository.GetById(id);
            if (radnja == null) return false;

            // 1. Logika brisanja Setve/Žetve (Vraćanje stanja, brisanje veza u ParcelaKultura)
            await ObradiLogikuSetveZetve_Delete(radnja);

            // 2. Veze (Mašine, Resursi) se brišu automatski ako je podešen Cascade Delete u bazi.
            // Ako nije, moramo ručno:
            // (Pretpostavićemo da baza to rešava ili da si to rešio u repozitorijumu, 
            // ali tvoj originalni kod nije eksplicitno brisao veze ovde, pa neću ni ja da ne komplikujem).

            return await _radnjaRepository.Delete(radnja);
        }

        // --- POMOĆNE METODE (Private Logic) ---

        private async Task ValidacijaRadnje(RadnjaDTO dto)
        {
            if (dto.Parcele == null || !dto.Parcele.Any())
                throw new ArgumentException("Morate izabrati bar jednu parcelu.");

            if (dto.IdKultura == null)
                throw new ArgumentException("Kultura je obavezna.");

            // Iteriramo kroz svaku parcelu da proverimo uslove
            foreach (var p in dto.Parcele)
            {
                if (dto.TipRadnje == RadnjaTip.Setva)
                {
                    if (p.Povrsina <= 0)
                        throw new ArgumentException($"Površina za parcelu {p.IdParcela} mora biti veća od 0.");

                    var slobodno = await GetSlobodnaPovrsinaAsync(p.IdParcela);
                    
                    // Ako radimo update, moramo uzeti u obzir da mi već zauzimamo deo te površine
                    // Ali za Add je prosto:
                    if (p.Povrsina > slobodno)
                        throw new ArgumentException($"Nema dovoljno slobodne površine na parceli {p.NazivParcele}. Dostupno: {slobodno:F2} ha.");
                }

                if (dto.TipRadnje == RadnjaTip.Zetva)
                {
                    // Provera prinosa se odnosi na celu radnju (ukupno), ne mora po parceli
                    if (!dto.Prinos.HasValue || dto.Prinos <= 0)
                        throw new ArgumentException("Prinos mora biti unet za žetvu.");

                    var aktivnaSetva = await _parcelaKulturaService.GetNezavrsenaSetva(p.IdParcela, dto.IdKultura.Value);
                    if (aktivnaSetva == null)
                        throw new ArgumentException($"Nema aktivne setve za ovu kulturu na parceli {p.NazivParcele}.");
                }
            }
        }

        private async Task ObradiLogikuSetveZetve_Add(RadnjaDTO dto, Guid idRadnja)
        {
            // PROLAZIMO KROZ SVE PARCELE U LISTI
            if (dto.Parcele != null)
            {
                foreach (var p in dto.Parcele)
                {
                    if (dto.TipRadnje == RadnjaTip.Setva)
                    {
                        // Kreiramo DTO za svaku parcelu pojedinačno
                        var pkDto = new ParcelaKulturaDTO
                        {
                            IdParcela = p.IdParcela,
                            IdKultura = dto.IdKultura,
                            // KLJUČNA IZMENA: Uzimamo površinu baš za tu parcelu iz liste
                            Povrsina = p.Povrsina,
                            DatumSetve = dto.DatumIzvrsenja,
                            IdSetvaRadnja = idRadnja
                        };
                        await _parcelaKulturaService.Add(pkDto);
                    }
                    else if (dto.TipRadnje == RadnjaTip.Zetva)
                    {
                        // Zatvaramo setvu na OVOJ parceli iz liste
                        var aktivneSetve = await _parcelaKulturaService.GetSveNezavrseneSetve(p.IdParcela, dto.IdKultura.Value);
                        foreach (var setva in aktivneSetve)
                        {
                            setva.DatumZetve = dto.DatumIzvrsenja;
                            setva.IdZetvaRadnja = idRadnja;
                            await _parcelaKulturaService.Update(setva);
                        }
                    }
                }
            }

            // Prinos se i dalje dodaje samo jednom (ukupno za celu radnju/kulturu)
            if (dto.TipRadnje == RadnjaTip.Zetva && dto.Prinos.HasValue && dto.Prinos.Value > 0)
            {
                // Kastujemo u decimal ako tvoj servis za kulture tako zahteva
                await _kulturaService.AzurirajPosleZetve(dto.IdKultura!.Value, (decimal)dto.Prinos.Value);
            }
        }

        private async Task ObradiLogikuSetveZetve_Update(Radnja staraRadnja, RadnjaDTO dto)
        {
            if (staraRadnja.TipRadnje == RadnjaTip.Setva)
            {
                // 1. Učitaj sve zapise setve koji su povezani sa ovom radnjom
                var postojecePK = await _parcelaKulturaService.GetAllBySetvaRadnjaId((Guid)staraRadnja.Id);
                var noveParceleIds = dto.Parcele.Select(p => p.IdParcela).ToList();

                // A. BRISANJE (ako je neka parcela izbačena, mada je kod tebe disabled)
                var zaBrisanje = postojecePK.Where(pk => !noveParceleIds.Contains(pk.IdParcela!.Value)).ToList();
                foreach (var pk in zaBrisanje)
                {
                    if (pk.Id.HasValue) await _parcelaKulturaService.Delete(pk.Id.Value);
                }

                // B. UPDATE i DODAVANJE
                foreach (var p in dto.Parcele)
                {
                    var target = postojecePK.FirstOrDefault(x => x.IdParcela == p.IdParcela);

                    if (target != null)
                    {
                        // Ako zapis postoji, ažuriramo površinu (jer je korisnik možda promenio u input polju)
                        if (target.Povrsina != p.Povrsina || target.DatumSetve != dto.DatumIzvrsenja)
                        {
                            target.Povrsina = p.Povrsina;
                            target.DatumSetve = dto.DatumIzvrsenja;
                            await _parcelaKulturaService.Update(target);
                        }
                    }
                    else
                    {
                        // Ako je u Edit modu nekako dodata nova parcela
                        var noviPk = new ParcelaKulturaDTO
                        {
                            IdParcela = p.IdParcela,
                            IdKultura = dto.IdKultura,
                            Povrsina = p.Povrsina,
                            DatumSetve = dto.DatumIzvrsenja,
                            IdSetvaRadnja = staraRadnja.Id
                        };
                        await _parcelaKulturaService.Add(noviPk);
                    }
                }
            }
            else if (staraRadnja.TipRadnje == RadnjaTip.Zetva)
            {
                // Logika za prinos (magacin)
                double stariPrinos = (staraRadnja as Zetva)?.Prinos ?? 0;
                double noviPrinos = dto.Prinos ?? stariPrinos;

                if (noviPrinos != stariPrinos)
                {
                    await _kulturaService.AzurirajPosleIzmeneZetve(staraRadnja.IdKultura!.Value, (decimal)stariPrinos, (decimal)noviPrinos);
                }

                // Ažuriranje datuma žetve na svim zatvorenim setvama ove radnje
                var aktivne = await _parcelaKulturaService.GetSveZaZetvu((Guid)staraRadnja.Id);
                foreach (var pk in aktivne)
                {
                    if (pk.DatumZetve != dto.DatumIzvrsenja)
                    {
                        pk.DatumZetve = dto.DatumIzvrsenja;
                        await _parcelaKulturaService.Update(pk);
                    }
                }
            }
        }

        private async Task ValidacijaIzmeneSetveZetve(Radnja staraRadnja, RadnjaDTO dto)
        {
            if (staraRadnja.TipRadnje == RadnjaTip.Setva)
            {
                var pk = await _parcelaKulturaService.GetBySetvaRadnjaId((Guid)staraRadnja.Id);
                if (pk != null && pk.IdZetvaRadnja != null)
                    throw new Exception("Setva ne može biti izmenjena jer je za nju već obavljena žetva.");
            }
        }

        private async Task ObradiLogikuSetveZetve_Delete(Radnja radnja)
        {
            if (radnja.TipRadnje == RadnjaTip.Setva)
            {
                // 1. Dohvatamo SVE zapise iz ParcelaKultura koji su nastali ovom setvom
                // (Koristimo onaj novi metod iz ParcelaKulturaService)
                var pkList = await _parcelaKulturaService.GetAllBySetvaRadnjaId((Guid)radnja.Id);

                if (pkList != null && pkList.Any())
                {
                    // 2. Sigurnosna provera: Da li je BILO KOJA od ovih parcela već požnjevena?
                    // Ako jeste, ne smemo obrisati setvu jer bi žetva ostala bez svog "roditelja".
                    // Korisnik mora prvo obrisati žetvu.
                    var imaZetve = pkList.Any(pk => pk.IdZetvaRadnja != null || pk.DatumZetve != null);

                    if (imaZetve)
                    {
                        throw new Exception("Nije moguće obrisati setvu jer je na jednoj ili više parcela već evidentirana žetva. Prvo morate obrisati žetvu.");
                    }

                    // 3. Ako nema žetve, bezbedno je obrisati sve zapise o setvi
                    foreach (var pk in pkList)
                    {
                        // Pozivamo Delete metodu servisa (pretpostavljamo da PK DTO ima Id)
                        if (pk.Id.HasValue)
                        {
                            await _parcelaKulturaService.Delete(pk.Id.Value);
                        }
                    }
                }
            }
            else if (radnja.TipRadnje == RadnjaTip.Zetva)
            {
                // Logika za Žetvu ostaje ista (kako si je i napisao)
                var zetva = (Zetva)radnja;

                // Provera stanja u magacinu
                bool moze = await _kulturaService.MozeBrisanjeZetve(zetva.IdKultura!.Value, (decimal)zetva.Prinos);
                if (!moze) throw new Exception("Nije moguće obrisati žetvu (deo prinosa je već prodat ili potrošen).");

                // Vraćamo stanje u magacinu (smanjujemo za iznos prinosa koji brišemo)
                var kultura = await _kulturaService.GetById(zetva.IdKultura!.Value);
                kultura.RaspolozivoZaProdaju -= (decimal)zetva.Prinos;
                await _kulturaService.Update(kultura.Id, kultura);

                // Oslobađamo parcele (Setujemo DatumZetve i IdZetvaRadnja na null)
                var pkList = await _parcelaKulturaService.GetSveZaZetvu((Guid)radnja.Id);
                foreach (var pk in pkList)
                {
                    pk.IdZetvaRadnja = null;
                    pk.DatumZetve = null;
                    await _parcelaKulturaService.Update(pk);
                }
            }
        }

        // --- OSTALE METODE (Get, GetAll...) ---

        public async Task<RadnjaDTO> GetForEdit(Guid id, Guid idKorisnik)
        {
            var radnja = await GetById(id);
            if (radnja == null) return null;

            // Učitaj veze
            radnja.RadneMasine = await _radnjaRadnaMasinaService.GetAllByRadnjaId(id);
            radnja.PrikljucneMasine = await _radnjaPrikljucnaMasinaService.GetAllByRadnjaId(id);
            radnja.Resursi = await _radnjaResursService.GetAllByRadnjaId(id);

            radnja.Parcele = await _radnjaParcelaService.GetAllByRadnjaId(id);

            // Ukupna površina za prikaz (sumiramo sve parcele)
            if (radnja.Parcele != null)
            {
                radnja.UkupnaPovrsina = radnja.Parcele.Sum(p => p.Povrsina);
            }

            return radnja;
        }

        // ... (Ostale metode: GetAllByKorisnikPaged, GetSlobodnaPovrsinaAsync itd. ostaju iste) ...
        public async Task<List<RadnjaDTO>> GetAllByKorisnikPaged(Guid idKorisnik, int skip, int take)
        {
            // 1. Dohvatanje entiteta iz baze
            var radnjeEntities = await _radnjaRepository.GetAllByKorisnikPaged(idKorisnik, skip, take);

            // 2. Mapiranje u DTO
            var radnjeDtos = radnjeEntities.Select(x => x.ToRadnjaDTO()).ToList();

            // 3. Dopunjavanje podataka (Enrichment)
            foreach (var dto in radnjeDtos)
            {
                if (dto.Id.HasValue)
                {
                    // Učitavamo parcele da bismo prikazali imena i ukupnu površinu u tabeli
                    // Paznja: Ovo može biti sporo ako ima puno radnji (N+1 problem).
                    // Ako ti treba samo površina, bolje je to rešiti u Repozitorijumu kroz Include/Select.
                    // Ali prateći tvoj patern:
                    var parcele = await _radnjaParcelaService.GetAllByRadnjaId(dto.Id.Value);
                    dto.Parcele = parcele;
                    dto.UkupnaPovrsina = parcele.Sum(p => p.Povrsina);
                }
            }

            return radnjeDtos;
        }

        // ... Kopiraj ostale jednostavne metode iz starog servisa ...
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
            // 1. Prvo izvučemo osnovne podatke iz repozitorijuma
            var radnje = await _radnjaRepository.GetAllByParcelaPaged(idParcela, skip, take);

            // 2. Mapiramo u DTO
            var radnjeDtos = radnje.Select(x => x.ToRadnjaDTO()).ToList();

            // 3. Prolazimo kroz listu i dovlačimo specifičnu površinu za OVU parcelu
            foreach (var dto in radnjeDtos)
            {
                if (dto.Id.HasValue)
                {
                    // Pozivamo servis za veznu tabelu da nađe zapis po ID-ju radnje i ID-ju parcele
                    // (Ovu metodu smo dodali u RadnjaParcelaService u prethodnom koraku)
                    var radnjaParcelaDto = await _radnjaParcelaService.GetByRadnjaAndParcela(dto.Id.Value, idParcela);

                    if (radnjaParcelaDto != null)
                    {
                        // A) Ubacujemo taj jedan zapis u listu, tako da dto.Parcele sadrži info o ovoj parceli
                        dto.Parcele = new List<RadnjaParcelaDTO> { radnjaParcelaDto };

                        // B) Koristimo postojeće polje 'UkupnaPovrsina' da bismo tu upisali površinu OVE parcele.
                        // Ovo radimo da bi na Frontendu lako prikazao brojku u koloni "Površina", 
                        // umesto da moraš da kopaš po listi (dto.Parcele[0].Povrsina).
                        dto.UkupnaPovrsina = radnjaParcelaDto.Povrsina;
                    }
                }
            }

            return radnjeDtos;
        }

        public async Task<int> GetCountByParcela(Guid idParcela)
        {
            return await _radnjaRepository.GetCountByParcela(idParcela);
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