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
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new RadnaMasinaDTO()); // Prazan model za dodavanje
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(RadnaMasinaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

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
            catch (ArgumentException)
            {
                ModelState.AddModelError("Naziv", "Već postoji radna mašina sa ovim nazivom za vaš nalog.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Došlo je do greške pri čuvanju. Proverite da su svi podaci ispravni.");
            }

            ViewBag.UserId = userId;
            return View(dto);
        }


        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var radnaMasina = await _radnaMasinaService.GetById(id);
            if (radnaMasina == null) return NotFound();

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View("Dodaj", radnaMasina);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnaMasinaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            dto.IdKorisnik = Guid.Parse(userId);
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = userId;
                return View("Dodaj", dto);
            }

            try
            {
                await _radnaMasinaService.Update(dto.Id, dto);
                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("RadneMasine");
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Naziv", "Već postoji radna mašina sa ovim nazivom za vaš nalog.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju. Proverite unos.");
            }

            ViewBag.UserId = userId;
            return View("Dodaj", dto);
        }


        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _radnaMasinaService.DeleteById(id);
            return RedirectToAction("RadneMasine");
        }

    }
}
