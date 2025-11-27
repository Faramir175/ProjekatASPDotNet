using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("prikljucne-masine")]
    public class PrikljucnaMasinaController : Controller
    {
        private readonly IPrikljucnaMasinaService _prikljucnaMasinaService;

        public PrikljucnaMasinaController(IPrikljucnaMasinaService prikljucnaMasinaService)
        {
            _prikljucnaMasinaService = prikljucnaMasinaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> PrikljucneMasine(int skip = 0, int take = 9)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var masine = await _prikljucnaMasinaService.GetAllByKorisnikPaged(idKorisnik, skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _prikljucnaMasinaService.GetCountByKorisnik(idKorisnik);

            return View(masine);
        }

        [HttpGet("dodaj")]
        public IActionResult Dodaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            ViewBag.UserId = userId;
            return View(new PrikljucnaMasinaDTO());
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(PrikljucnaMasinaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            dto.IdKorisnik = Guid.Parse(userId);

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                return View(dto);
            }

            try
            {
                await _prikljucnaMasinaService.Add(dto);
                TempData["SuccessMessage"] = "Priključna mašina je uspešno dodata!";
                return RedirectToAction("PrikljucneMasine");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Naziv", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Došlo je do greške pri čuvanju. Proverite unos.");
            }

            ViewBag.UserId = userId;
            return View(dto);
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            Guid idKorisnik = Guid.Parse(userId);

            var masina = await _prikljucnaMasinaService.GetById(id);
            if (masina == null) return NotFound();

            // SIGURNOSNA PROVERA VLASNIŠTVA
            if (masina.IdKorisnik != idKorisnik) return Unauthorized();

            ViewBag.UserId = userId;
            return View("Dodaj", masina);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, PrikljucnaMasinaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            Guid idKorisnik = Guid.Parse(userId);

            dto.IdKorisnik = idKorisnik;
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                return View("Dodaj", dto);
            }

            try
            {
                var updated = await _prikljucnaMasinaService.Update(dto.Id, dto);

                if (updated == null) return NotFound();

                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("PrikljucneMasine");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Naziv", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju.");
            }

            ViewBag.UserId = userId;
            return View("Dodaj", dto);
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            Guid idKorisnik = Guid.Parse(userId);

            // 1. Provera vlasništva
            var masina = await _prikljucnaMasinaService.GetById(id);
            if (masina == null) return NotFound();

            if (masina.IdKorisnik != idKorisnik) return Unauthorized();

            // 2. Brisanje
            try
            {
                await _prikljucnaMasinaService.DeleteById(id);
                TempData["SuccessMessage"] = "Priključna mašina je obrisana.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Greška pri brisanju.";
            }

            return RedirectToAction("PrikljucneMasine");
        }
    }
}