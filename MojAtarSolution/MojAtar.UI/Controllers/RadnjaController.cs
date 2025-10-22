﻿using Microsoft.AspNetCore.Mvc;
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


        public RadnjaController(IRadnjaService radnjaService, IKulturaService kulturaService,
            IParcelaService parcelaService, IRadnjaRadnaMasinaService radnjaRadnaMasinaService, 
            IRadnaMasinaService radnaMasinaService, IRadnjaPrikljucnaMasinaService radnjaPrikljucnaMasinaService,
            IPrikljucnaMasinaService prikljucnaMasinaService, IResursService resursService, 
            IRadnjaResursService radnjaResursService, IParcelaKulturaService parcelaKulturaService)
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
                    radnja.IdParcela.HasValue &&
                    radnja.IdKultura.HasValue)
                {
                    var parcelaKultura = await _parcelaKulturaService.GetByParcelaAndKulturaId(
                        radnja.IdParcela.Value, radnja.IdKultura.Value);

                    if (parcelaKultura != null &&
                        parcelaKultura.DatumSetve != null &&
                        parcelaKultura.DatumSetve == radnja.DatumIzvrsenja)
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
                    radnja.IdParcela.HasValue &&
                    radnja.IdKultura.HasValue)
                {
                    var parcelaKultura = await _parcelaKulturaService.GetByParcelaAndKulturaId(
                        radnja.IdParcela.Value, radnja.IdKultura.Value);

                    if (parcelaKultura != null &&
                        parcelaKultura.DatumSetve != null &&
                        parcelaKultura.DatumSetve == radnja.DatumIzvrsenja)
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
                return View(dto);

            if (!await ObradiParcelaKulturaAsync(dto))
            {
                Guid idKorisnik = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var kulture = await _kulturaService.GetAllForUser(idKorisnik);
                var parcele = await _parcelaService.GetAllForUser(idKorisnik);
                var radneMasine = await _radnaMasinaService.GetAllForUser(idKorisnik);
                var prikljucneMasine = await _prikljucnaMasinaService.GetAllForUser(idKorisnik);
                var resursi = await _resursService.GetAllForUser(idKorisnik);

                ViewBag.KultureSelectList = new SelectList(kulture, "Id", "Naziv");
                ViewBag.ParceleSelectList = new SelectList(parcele, "Id", "Naziv");
                ViewBag.RadneMasineSelectList = new SelectList(radneMasine, "Id", "Naziv");
                ViewBag.PrikljucneMasineSelectList = new SelectList(prikljucneMasine, "Id", "Naziv");
                ViewBag.ResursiSelectList = new SelectList(resursi, "Id", "Naziv");

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
                await _radnjaResursService.Add(resurs);
            }

            return RedirectToAction("Radnje");
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

            var ceneResursa = resursi.ToDictionary(r => r.Id.ToString(), r => r.AktuelnaCena);
            ViewBag.CeneResursa = ceneResursa;

            ViewBag.KultureSelectList = new SelectList(kulture, "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(parcele, "Id", "Naziv");
            ViewBag.RadneMasineSelectList = new SelectList(radneMasine, "Id", "Naziv");
            ViewBag.PrikljucneMasineSelectList = new SelectList(prikljucneMasine, "Id", "Naziv");
            ViewBag.ResursiSelectList = new SelectList(resursi, "Id", "Naziv");

            if (radnja.TipRadnje == RadnjaTip.Setva)
            {
                var parcelaKultura = await _parcelaKulturaService
                    .GetByParcelaAndKulturaId(radnja.IdParcela.Value, radnja.IdKultura.Value);

                if (parcelaKultura != null)
                {
                    radnja.Povrsina = parcelaKultura.Povrsina;
                }
            }

            radnja.RadneMasine = povezaneMasine;
            radnja.PrikljucneMasine = povezanePrikljucne;
            radnja.Resursi = povezaniResursi;

            return View("Dodaj",radnja);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnjaDTO dto, List<Guid> ObrisaneRadneMasineId, List<Guid> ObrisanePrikljucneMasineId, List<Guid> ObrisaniResursiId)
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

            if (!ModelState.IsValid)
                return View(dto);

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

            return RedirectToAction("Radnje");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _radnjaService.DeleteById(id);
            return RedirectToAction("Radnje");
        }

        private async Task<bool> ObradiParcelaKulturaAsync(RadnjaDTO dto)
        {
            if (dto.IdParcela != null && dto.IdKultura != null)
            {
                var parcelaId = dto.IdParcela.Value;
                var kulturaId = dto.IdKultura.Value;

                if (dto.TipRadnje == RadnjaTip.Setva)
                {
                    var unosSetva = new ParcelaKulturaDTO
                    {
                        Id = Guid.NewGuid(),
                        IdParcela = parcelaId,
                        IdKultura = kulturaId,
                        DatumSetve = dto.DatumIzvrsenja,
                        Povrsina = dto.Povrsina ?? 0,
                        IdKorisnik = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
                    };

                    await _parcelaKulturaService.Add(unosSetva);
                }
                else if (dto.TipRadnje == RadnjaTip.Zetva)
                {
                    var unosZetva = await _parcelaKulturaService.GetNezavrsenaSetva(parcelaId, kulturaId);
                    if (unosZetva != null)
                    {
                        unosZetva.DatumZetve = dto.DatumIzvrsenja;
                        await _parcelaKulturaService.Update(unosZetva);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Zetva nije moguća jer kultura nije posejana na ovoj parceli.");
                        return false;
                    }

                }
            }
            return true;
        }

    }
}