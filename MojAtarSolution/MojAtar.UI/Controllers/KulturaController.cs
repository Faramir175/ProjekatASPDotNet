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
    [Route("kulture")]
    public class KulturaController : Controller
    {
        private readonly IKulturaService _kulturaService;

        public KulturaController(IKulturaService kulturaService)
        {
            _kulturaService = kulturaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Kulture(int skip = 0, int take = 9)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var kulture = await _kulturaService.GetAllByKorisnikPaged(idKorisnik, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _kulturaService.GetCountByKorisnik(idKorisnik);

            return View(kulture);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new KulturaDTO()); // Prazan model za dodavanje
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(KulturaDTO dto)
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
                await _kulturaService.Add(dto);
                TempData["SuccessMessage"] = "Kultura je uspešno dodata!";
                return RedirectToAction("Kulture");
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Naziv", "Već postoji kultura sa ovim nazivom za vaš nalog.");
            }
            catch (DbUpdateException)
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
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var kultura = await _kulturaService.GetWithAktuelnaCena(idKorisnik, id);
            if (kultura == null) return NotFound();

            ViewBag.UserId = userId;
            return View("Dodaj", kultura);
        }


        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, KulturaDTO dto)
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
                await _kulturaService.Update(dto.Id, dto);
                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("Kulture");
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("Naziv", "Već postoji kultura sa ovim nazivom za vaš nalog.");
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
            await _kulturaService.DeleteById(id);
            return RedirectToAction("Kulture");
        }

    }
}
