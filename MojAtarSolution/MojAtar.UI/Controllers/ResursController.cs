using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("resursi")]
    public class ResursController : Controller
    {
        private readonly IResursService _resursService;

        public ResursController(IResursService resursService)
        {
            _resursService = resursService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Resursi(int skip = 0, int take = 9)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var resursi = await _resursService.GetAllByKorisnikPaged(idKorisnik, skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _resursService.GetCountByKorisnik(idKorisnik);

            return View(resursi);
        }

        [HttpGet("dodaj")]
        public IActionResult Dodaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            ViewBag.UserId = userId;
            return View(new ResursDTO());
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(ResursDTO dto)
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
                await _resursService.Add(dto);
                TempData["SuccessMessage"] = "Resurs je uspešno dodat!";
                return RedirectToAction("Resursi");
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

            // Koristimo metodu koja vraća i aktuelnu cenu
            var resurs = await _resursService.GetWithAktuelnaCena(idKorisnik, id);

            if (resurs == null) return NotFound();

            // PROVERA VLASNIŠTVA
            if (resurs.IdKorisnik != idKorisnik) return Unauthorized();

            ViewBag.UserId = userId;
            return View("Dodaj", resurs);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, ResursDTO dto)
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
                var existing = await _resursService.GetById(id);
                if (existing == null || existing.IdKorisnik != idKorisnik) return Unauthorized();

                await _resursService.Update(dto.Id, dto);
                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("Resursi");
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

            // 1. Provera
            var resurs = await _resursService.GetById(id);
            if (resurs == null) return NotFound();

            // 2. Vlasništvo
            if (resurs.IdKorisnik != idKorisnik) return Unauthorized();

            // 3. Brisanje
            await _resursService.DeleteById(id);

            TempData["SuccessMessage"] = "Resurs je uspešno obrisan.";
            return RedirectToAction("Resursi");
        }
    }
}