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

        // Pomoćna metoda za izvlačenje ID-a ulogovanog korisnika
        private Guid GetUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) throw new UnauthorizedAccessException();
            return Guid.Parse(userIdStr);
        }

        [HttpGet("")]
        public async Task<IActionResult> Kulture(int skip = 0, int take = 9)
        {
            try
            {
                Guid idKorisnik = GetUserId();
                var kulture = await _kulturaService.GetAllByKorisnikPaged(idKorisnik, skip, take);

                ViewBag.Skip = skip + take;
                ViewBag.TotalCount = await _kulturaService.GetCountByKorisnik(idKorisnik);

                return View(kulture);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet("dodaj")]
        public IActionResult Dodaj()
        {
            try
            {
                ViewBag.UserId = GetUserId().ToString(); 
                return View(new KulturaDTO());
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(KulturaDTO dto)
        {
            dto.IdKorisnik = GetUserId();
            ViewBag.UserId = dto.IdKorisnik.ToString();

            if (!ModelState.IsValid)
            {
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

            
            return View(dto);
        }


        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            try
            {
                Guid idKorisnik = GetUserId();

                var kultura = await _kulturaService.GetWithAktuelnaCena(idKorisnik, id);

                if (kultura == null) return NotFound();

                if (kultura.IdKorisnik != idKorisnik) return Unauthorized();

                ViewBag.UserId = idKorisnik.ToString();
                return View("Dodaj", kultura);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login");
            }
        }


        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, KulturaDTO dto)
        {
            dto.IdKorisnik = GetUserId();
            dto.Id = id;

            ViewBag.UserId = dto.IdKorisnik.ToString();

            if (!ModelState.IsValid)
            {
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

            
            return View("Dodaj", dto);
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            try
            {
                Guid idKorisnik = GetUserId(); 

                var kultura = await _kulturaService.GetById(id);
                if (kultura == null) return NotFound();

                if (kultura.IdKorisnik != idKorisnik)
                {
                    return Unauthorized(); 
                }

                // 3. Ako je sve ok, briši
                await _kulturaService.DeleteById(id);
                TempData["SuccessMessage"] = "Kultura je obrisana.";
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Greška pri brisanju.";
            }
            return RedirectToAction(nameof(Kulture));
        }
    }
}
