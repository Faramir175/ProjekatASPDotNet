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
        private readonly IRadnjaRadnaMasinaService _radnjaRadnaMasinaService;
        public RadnjaController(IRadnjaService radnjaService, IKulturaService kulturaService,IParcelaService parcelaService, IRadnjaRadnaMasinaService radnjaRadnaMasinaService, IRadnaMasinaService radnaMasinaService)
        {
            _radnjaService = radnjaService;
            _kulturaService = kulturaService;
            _parcelaService = parcelaService;
            _radnjaRadnaMasinaService = radnjaRadnaMasinaService;
            _radnaMasinaService = radnaMasinaService;
        }

        // Prikaz poslednjih 10 radnji korisnika
        [HttpGet("")]
        public async Task<IActionResult> Radnje(int skip = 0, int take = 10)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var radnje = await _radnjaService.GetAllByKorisnikPaged(idKorisnik, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _radnjaService.GetCountByKorisnik(idKorisnik);

            return View(radnje);
        }


        // Prikaz radnji za konkretnu parcelu
        [HttpGet("RadnjePoParceli/{idParcela}")]
        public async Task<IActionResult> RadnjePoParceli(Guid idParcela, int skip = 0, int take = 10)
        {
            var radnje = await _radnjaService.GetAllByParcelaPaged(idParcela, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.IdParcela = idParcela;
            ViewBag.TotalCount = await _radnjaService.GetCountByParcela(idParcela);

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

            ViewBag.RadneMasineSelectList = new SelectList(radneMasine, "Id", "Naziv");
            ViewBag.KultureSelectList = new SelectList(kulture, "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(parcele, "Id", "Naziv");

            return View(new RadnjaDTO());
        }


        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(RadnjaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var novaRadnja = await _radnjaService.Add(dto);

            foreach (var radnjaMasina in dto.RadneMasine)
            {
                radnjaMasina.IdRadnja = (Guid)novaRadnja.Id;
                await _radnjaRadnaMasinaService.Add(radnjaMasina);
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

            ViewBag.KultureSelectList = new SelectList(kulture, "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(parcele, "Id", "Naziv");
            ViewBag.RadneMasineSelectList = new SelectList(radneMasine, "Id", "Naziv");

            radnja.RadneMasine = povezaneMasine;

            return View("Dodaj",radnja);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnjaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            foreach (var masina in dto.RadneMasine)
            {
                await _radnjaRadnaMasinaService.Delete(id, masina.IdRadnaMasina);
            }

            foreach (var masina in dto.RadneMasine)
            {
                var dtoMasina = new RadnjaRadnaMasinaDTO
                {
                    IdRadnja = id,
                    IdRadnaMasina = masina.IdRadnaMasina,
                    BrojRadnihSati = masina.BrojRadnihSati
                };
                await _radnjaRadnaMasinaService.Add(dtoMasina);
            }

            // Ažuriraj radnju sa novim podacima
            await _radnjaService.Update(id, dto);

            return RedirectToAction("Radnje");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _radnjaService.DeleteById(id);
            return RedirectToAction("Radnje");
        }
    }
}
