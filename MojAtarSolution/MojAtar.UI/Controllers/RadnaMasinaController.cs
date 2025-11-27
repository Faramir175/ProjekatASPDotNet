using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("radne-masine")]
    public class RadnaMasinaController : Controller
    {
        private readonly IRadnaMasinaService _radnaMasinaService;

        public RadnaMasinaController(IRadnaMasinaService radnaMasinaService)
        {
            _radnaMasinaService = radnaMasinaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> RadneMasine(int skip = 0, int take = 9)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var masine = await _radnaMasinaService.GetAllByKorisnikPaged(idKorisnik, skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _radnaMasinaService.GetCountByKorisnik(idKorisnik);

            return View(masine);
        }

        [HttpGet("dodaj")]
        public IActionResult Dodaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            ViewBag.UserId = userId;
            return View(new RadnaMasinaDTO());
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(RadnaMasinaDTO dto)
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
                await _radnaMasinaService.Add(dto);
                TempData["SuccessMessage"] = "Radna mašina je uspešno dodata!";
                return RedirectToAction("RadneMasine");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Naziv", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Došlo je do greške pri čuvanju.");
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

            var radnaMasina = await _radnaMasinaService.GetById(id);
            if (radnaMasina == null) return NotFound();

            // SIGURNOSNA PROVERA VLASNIŠTVA
            if (radnaMasina.IdKorisnik != idKorisnik) return Unauthorized();

            ViewBag.UserId = userId;
            return View("Dodaj", radnaMasina);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnaMasinaDTO dto)
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
                var updated = await _radnaMasinaService.Update(dto.Id, dto);

                if (updated == null) return NotFound();

                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("RadneMasine");
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

            // 1. Dohvatanje radi provere
            var masina = await _radnaMasinaService.GetById(id);
            if (masina == null) return NotFound();

            // 2. Provera vlasništva
            if (masina.IdKorisnik != idKorisnik) return Unauthorized();

            // 3. Brisanje
            await _radnaMasinaService.DeleteById(id);

            TempData["SuccessMessage"] = "Radna mašina je obrisana.";
            return RedirectToAction("RadneMasine");
        }
    }
}