using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Services;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("radnje")]
    public class RadnjaController : Controller
    {
        private readonly IRadnjaService _radnjaService;
        private readonly IKulturaService _kulturaService; 
        private readonly IParcelaService _parcelaService;
        private readonly IRadnaMasinaService _radnaMasinaService;
        private readonly IPrikljucnaMasinaService _prikljucnaMasinaService;
        private readonly IResursService _resursService;
        private readonly IRadnjaRadnaMasinaService _radnjaRadnaMasinaService;
        private readonly IRadnjaPrikljucnaMasinaService _radnjaPrikljucnaMasinaService;
        private readonly IRadnjaResursService _radnjaResursService;
        private readonly IParcelaKulturaService _parcelaKulturaService;
        private readonly ICenaResursaService _cenaResursaService;


        public RadnjaController(IRadnjaService radnjaService, IKulturaService kulturaService,
            IParcelaService parcelaService, IRadnjaRadnaMasinaService radnjaRadnaMasinaService, 
            IRadnaMasinaService radnaMasinaService, IRadnjaPrikljucnaMasinaService radnjaPrikljucnaMasinaService,
            IPrikljucnaMasinaService prikljucnaMasinaService, IResursService resursService, 
            IRadnjaResursService radnjaResursService, IParcelaKulturaService parcelaKulturaService, 
            ICenaResursaService cenaResursaService)
        {
            _radnjaService = radnjaService;
            _kulturaService = kulturaService;
            _parcelaService = parcelaService;
            _radnjaRadnaMasinaService = radnjaRadnaMasinaService;
            _radnaMasinaService = radnaMasinaService;
            _radnjaPrikljucnaMasinaService = radnjaPrikljucnaMasinaService;
            _prikljucnaMasinaService = prikljucnaMasinaService;
            _resursService = resursService;
            _radnjaResursService = radnjaResursService;
            _parcelaKulturaService = parcelaKulturaService;
            _cenaResursaService = cenaResursaService;
        }

        // Prikaz poslednjih 10 radnji korisnika
        [HttpGet("")]
        public async Task<IActionResult> Radnje(int skip = 0, int take = 9)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var radnje = await _radnjaService.GetAllByKorisnikPaged(idKorisnik, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _radnjaService.GetCountByKorisnik(idKorisnik);

            foreach (var radnja in radnje)
            {
                if (radnja.TipRadnje == RadnjaTip.Setva &&
                    radnja.Id != null &&
                    radnja.IdParcela.HasValue &&
                    radnja.IdKultura.HasValue)
                {
                    // pronađi tačno onu parcelu-kulturu koja koristi ovu setvu
                    var parcelaKultura = await _parcelaKulturaService.GetBySetvaRadnjaId(radnja.Id.Value);

                    if (parcelaKultura != null)
                    {
                        radnja.Povrsina = parcelaKultura.Povrsina;
                    }
                }
            }

            return View(radnje);
        }


        // Prikaz radnji za konkretnu parcelu
        [HttpGet("RadnjePoParceli/{idParcela}")]
        public async Task<IActionResult> RadnjePoParceli(Guid idParcela, int skip = 0, int take = 9)
        {
            var radnje = await _radnjaService.GetAllByParcelaPaged(idParcela, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.IdParcela = idParcela;
            ViewBag.TotalCount = await _radnjaService.GetCountByParcela(idParcela);

            foreach (var radnja in radnje)
            {
                if (radnja.TipRadnje == RadnjaTip.Setva &&
                    radnja.Id != null &&
                    radnja.IdParcela.HasValue &&
                    radnja.IdKultura.HasValue)
                {
                    // pronađi tačno onu parcelu-kulturu koja koristi ovu setvu
                    var parcelaKultura = await _parcelaKulturaService.GetBySetvaRadnjaId(radnja.Id.Value);

                    if (parcelaKultura != null)
                    {
                        radnja.Povrsina = parcelaKultura.Povrsina;
                    }
                }
            }

            return View(radnje);
        }


        // Dodavanje nove radnje
        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var kulture = await _kulturaService.GetAllForUser(idKorisnik);
            var parcele = await _parcelaService.GetAllForUser(idKorisnik);
            var radneMasine = await _radnaMasinaService.GetAllForUser(idKorisnik);
            var prikljucneMasine = await _prikljucnaMasinaService.GetAllForUser(idKorisnik);
            var resursi = await _resursService.GetAllForUser(idKorisnik);

            if (parcele.Any())
            {
                var prvaParcelaId = parcele.First().Id;
                ViewBag.SlobodnaPovrsina = await _radnjaService.GetSlobodnaPovrsinaAsync((Guid)prvaParcelaId);
            }

            var ceneResursa = resursi.ToDictionary(r => r.Id.ToString(), r => r.AktuelnaCena);
            ViewBag.CeneResursa = ceneResursa;


            ViewBag.KultureSelectList = new SelectList(kulture, "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(parcele, "Id", "Naziv");
            ViewBag.RadneMasineSelectList = new SelectList(radneMasine, "Id", "Naziv");
            ViewBag.PrikljucneMasineSelectList = new SelectList(prikljucneMasine, "Id", "Naziv");
            ViewBag.ResursiSelectList = new SelectList(resursi, "Id", "Naziv");

            return View(new RadnjaDTO());
        }


        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(RadnjaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await UcitajViewBagove();
                return View(dto);
            }
            if (dto.TipRadnje == RadnjaTip.Setva && (!dto.Povrsina.HasValue || dto.Povrsina <= 0))
                ModelState.AddModelError(nameof(dto.Povrsina), "Površina mora biti uneta za setvu.");

            if (dto.TipRadnje == RadnjaTip.Zetva && (!dto.Prinos.HasValue || dto.Prinos <= 0))
                ModelState.AddModelError(nameof(dto.Prinos), "Prinos mora biti unet za žetvu.");

            // Validacija povezanih mašina i resursa
            if (dto.RadneMasine != null)
            {
                foreach (var masina in dto.RadneMasine)
                {
                    if (masina.BrojRadnihSati <= 0)
                        ModelState.AddModelError("", "Morate uneti broj radnih sati za selektovanu mašinu.");
                }
            }

            if (dto.Resursi != null)
            {
                foreach (var resurs in dto.Resursi)
                {
                    if (resurs.Kolicina <= 0)
                        ModelState.AddModelError("", "Morate uneti količinu za selektovani resurs.");
                }
            }

            if (!ModelState.IsValid)
            {
                await UcitajViewBagove();
                return View(dto);
            }


            try
            {
                if (!await ObradiParcelaKulturaAsync(dto))
                {
                    await UcitajViewBagove();

                    return View(dto);
                }

                var novaRadnja = await _radnjaService.Add(dto);

                foreach (var radnjaMasina in dto.RadneMasine)
                {
                    radnjaMasina.IdRadnja = (Guid)novaRadnja.Id;
                    await _radnjaRadnaMasinaService.Add(radnjaMasina);
                }

                foreach (var prikljucna in dto.PrikljucneMasine)
                {
                    prikljucna.IdRadnja = (Guid)novaRadnja.Id;
                    await _radnjaPrikljucnaMasinaService.Add(prikljucna);
                }

                foreach (var resurs in dto.Resursi)
                {
                    resurs.IdRadnja = (Guid)novaRadnja.Id;
                    resurs.DatumKoriscenja = dto.DatumIzvrsenja;
                    await _radnjaResursService.Add(resurs);
                }

                TempData["SuccessMessage"] = "Radnja je uspešno dodata!";

                return RedirectToAction("Radnje");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);

                await UcitajViewBagove();
                return View(dto);
            }
        }


        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var radnja = await _radnjaService.GetById(id);

            if (radnja == null)
                return NotFound();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var kulture = await _kulturaService.GetAllForUser(idKorisnik);
            var parcele = await _parcelaService.GetAllForUser(idKorisnik);

            var radneMasine = await _radnaMasinaService.GetAllForUser(idKorisnik);
            var povezaneMasine = await _radnjaRadnaMasinaService.GetAllByRadnjaId(id);

            var prikljucneMasine = await _prikljucnaMasinaService.GetAllForUser(idKorisnik);
            var povezanePrikljucne = await _radnjaPrikljucnaMasinaService.GetAllByRadnjaId(id);

            var resursi = await _resursService.GetAllForUser(idKorisnik);
            var povezaniResursi = await _radnjaResursService.GetAllByRadnjaId(id);

            var ceneResursa = new Dictionary<string, double>();

            // Uzimamo cene koje su važile NA DATUM IZVRŠENJA radnje
            foreach (var res in resursi)
            {
                var cena = await _cenaResursaService.GetAktuelnaCena(idKorisnik, (Guid)res.Id, radnja.DatumIzvrsenja);
                ceneResursa[res.Id.ToString()] = cena;
            }

            ViewBag.CeneResursa = ceneResursa;


            ViewBag.KultureSelectList = new SelectList(kulture, "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(parcele, "Id", "Naziv");
            ViewBag.RadneMasineSelectList = new SelectList(radneMasine, "Id", "Naziv");
            ViewBag.PrikljucneMasineSelectList = new SelectList(prikljucneMasine, "Id", "Naziv");
            ViewBag.ResursiSelectList = new SelectList(resursi, "Id", "Naziv");

            bool setvaZakljucana = false;

            if (radnja.TipRadnje == RadnjaTip.Setva && radnja.Id != null)
            {
                // ⚡ Pronađi tačno onu ParcelaKultura koja koristi ovu setvu kao IdSetvaRadnja
                var parcelaKultura = await _parcelaKulturaService.GetBySetvaRadnjaId(radnja.Id.Value);

                if (parcelaKultura != null)
                {
                    radnja.Povrsina = parcelaKultura.Povrsina;

                    // Ako već ima povezanu žetvu — zaključaj
                    if (parcelaKultura.IdZetvaRadnja != null)
                        setvaZakljucana = true;
                }
            }


            radnja.RadneMasine = povezaneMasine;
            radnja.PrikljucneMasine = povezanePrikljucne;
            radnja.Resursi = povezaniResursi;

            ViewBag.SetvaZakljucana = setvaZakljucana;

            return View("Dodaj",radnja);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnjaDTO dto, List<Guid> ObrisaneRadneMasineId, List<Guid> ObrisanePrikljucneMasineId, List<Guid> ObrisaniResursiId)
        {
            if (dto.TipRadnje == RadnjaTip.Setva && (!dto.Povrsina.HasValue || dto.Povrsina <= 0))
                ModelState.AddModelError(nameof(dto.Povrsina), "Površina mora biti uneta za setvu.");

            if (dto.TipRadnje == RadnjaTip.Zetva && (!dto.Prinos.HasValue || dto.Prinos <= 0))
                ModelState.AddModelError(nameof(dto.Prinos), "Prinos mora biti unet za žetvu.");

            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                var parcelaKultura = await _parcelaKulturaService.GetBySetvaRadnjaId(id);
                if (parcelaKultura != null && parcelaKultura.IdZetvaRadnja != null)
                {
                    TempData["ErrorMessage"] = "Ova setva je zaključana jer je obavljena žetva. Izmena nije dozvoljena.";
                    return RedirectToAction("Izmeni", new { id });
                }
            }

            if (dto.RadneMasine != null)
            {
                foreach (var masina in dto.RadneMasine)
                {
                    if (masina.BrojRadnihSati <= 0)
                        ModelState.AddModelError("", "Morate uneti broj radnih sati za selektovanu mašinu.");
                }
            }

            if (dto.Resursi != null)
            {
                foreach (var resurs in dto.Resursi)
                {
                    if (resurs.Kolicina <= 0)
                        ModelState.AddModelError("", "Morate uneti količinu za selektovani resurs.");
                }
            }

            if (!ModelState.IsValid)
            {
                await UcitajViewBagove();
                return View("Dodaj", dto);
            }

            try
            {
                if (ObrisaneRadneMasineId != null)
                {
                    foreach (var idOM in ObrisaneRadneMasineId)
                    {
                        await _radnjaRadnaMasinaService.Delete(dto.Id.Value, idOM);
                    }
                }

                if (ObrisanePrikljucneMasineId != null)
                {
                    foreach (var idPM in ObrisanePrikljucneMasineId)
                        await _radnjaPrikljucnaMasinaService.Delete(dto.Id.Value, idPM);
                }

                if (ObrisaniResursiId != null)
                {
                    foreach (var idR in ObrisaniResursiId)
                        await _radnjaResursService.Delete(dto.Id.Value, idR);
                }

                foreach (var masina in dto.RadneMasine)
                {
                    await _radnjaRadnaMasinaService.Delete(id, masina.IdRadnaMasina);
                }

                foreach (var masina in dto.RadneMasine)
                {
                    masina.IdRadnja = id;
                    await _radnjaRadnaMasinaService.Add(masina);
                }

                foreach (var prikljucna in dto.PrikljucneMasine)
                {
                    await _radnjaPrikljucnaMasinaService.Delete(id, prikljucna.IdPrikljucnaMasina);
                }
                foreach (var prikljucna in dto.PrikljucneMasine)
                {
                    prikljucna.IdRadnja = id;
                    await _radnjaPrikljucnaMasinaService.Add(prikljucna);
                }

                foreach (var resurs in dto.Resursi)
                {
                    await _radnjaResursService.Delete(id, resurs.IdResurs);
                }
                foreach (var resurs in dto.Resursi)
                {
                    resurs.IdRadnja = id;
                    await _radnjaResursService.Add(resurs);
                }

                await _radnjaService.Update(id, dto);

                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("Radnje");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await UcitajViewBagove();
                return View("Dodaj", dto);
            }
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _radnjaService.DeleteById(id);
            return RedirectToAction("Radnje");
        }

        [HttpGet("slobodnaPovrsina/{idParcela}")]
        public async Task<IActionResult> GetSlobodnaPovrsina(Guid idParcela)
        {
            var slobodno = await _radnjaService.GetSlobodnaPovrsinaAsync(idParcela);
            return Json(new { slobodno });
        }


        private async Task<bool> ObradiParcelaKulturaAsync(RadnjaDTO dto)
        {
            if (dto.IdParcela == null || dto.IdKultura == null)
                return true;

            if (dto.TipRadnje == RadnjaTip.Setva)
            {
                return true;
            }
            else if (dto.TipRadnje == RadnjaTip.Zetva)
            {
                var aktivna = await _parcelaKulturaService.GetNezavrsenaSetva(
                    dto.IdParcela.Value, dto.IdKultura.Value);

                if (aktivna == null)
                {
                    ModelState.AddModelError("", "Žetva nije moguća jer kultura nije posejana.");
                    return false;
                }
            }
            return true;
        }

        private async Task UcitajViewBagove()
        {
            Guid idKorisnik = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.KultureSelectList = new SelectList(await _kulturaService.GetAllForUser(idKorisnik), "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(await _parcelaService.GetAllForUser(idKorisnik), "Id", "Naziv");
            ViewBag.RadneMasineSelectList = new SelectList(await _radnaMasinaService.GetAllForUser(idKorisnik), "Id", "Naziv");
            ViewBag.PrikljucneMasineSelectList = new SelectList(await _prikljucnaMasinaService.GetAllForUser(idKorisnik), "Id", "Naziv");
            ViewBag.ResursiSelectList = new SelectList(await _resursService.GetAllForUser(idKorisnik), "Id", "Naziv");
        }

    }


}