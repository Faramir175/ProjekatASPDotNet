using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("parcele")]
    public class ParcelaController : Controller
    {
        private readonly IParcelaService _parcelaService;
        private readonly IKatastarskaOpstinaService _katastarskaOpstinaService;

        public ParcelaController(IParcelaService parcelaService, IKatastarskaOpstinaService katastarskaOpstinaService)
        {
            _parcelaService = parcelaService;
            _katastarskaOpstinaService = katastarskaOpstinaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Parcele(int skip = 0, int take = 9)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var parcele = await _parcelaService.GetAllByKorisnikPagedWithActiveKulture(idKorisnik, skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _parcelaService.GetCountByKorisnik(idKorisnik);

            return View(parcele);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            ViewBag.KatastarskeOpstine = new SelectList(await _katastarskaOpstinaService.GetAll(), "Id", "Naziv");
            ViewBag.UserId = userId;
            
            return View(new ParcelaDTO()); 
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(ParcelaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            dto.IdKorisnik = Guid.Parse(userId);

            if (!ModelState.IsValid)
            {
                // Moramo ponovo napuniti ViewBag ako vraćamo View
                ViewBag.KatastarskeOpstine = new SelectList(
                    await _katastarskaOpstinaService.GetAll(), "Id", "Naziv", dto.IdKatastarskaOpstina);
                ViewBag.UserId = userId;
                return View(dto);
            }

            try
            {
                await _parcelaService.Add(dto);
                TempData["SuccessMessage"] = "Parcela je uspešno dodata!";
                return RedirectToAction("Parcele");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Naziv", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Došlo je do greške pri čuvanju.");
            }

            // Ako je došlo do greške, ponovo punimo liste
            ViewBag.KatastarskeOpstine = new SelectList(
                await _katastarskaOpstinaService.GetAll(), "Id", "Naziv", dto.IdKatastarskaOpstina);
            ViewBag.UserId = userId;

            return View(dto);
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            Guid idKorisnik = Guid.Parse(userId);

            var parcela = await _parcelaService.GetById(id);
            if (parcela == null) return NotFound();

            // SIGURNOSNA PROVERA
            if (parcela.IdKorisnik != idKorisnik) return Unauthorized();

            ViewBag.UserId = userId;
            ViewBag.KatastarskeOpstine = new SelectList(
                await _katastarskaOpstinaService.GetAll(), "Id", "Naziv", parcela.IdKatastarskaOpstina);
            
            return View("Dodaj", parcela);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, ParcelaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            Guid idKorisnik = Guid.Parse(userId);

            dto.IdKorisnik = idKorisnik;
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                ViewBag.KatastarskeOpstine = new SelectList(
                    await _katastarskaOpstinaService.GetAll(), "Id", "Naziv", dto.IdKatastarskaOpstina);
                ViewBag.UserId = userId;
                return View("Dodaj", dto);
            }

            try
            {
                // Servis radi update
                var updated = await _parcelaService.Update(dto.Id, dto);
                
                if (updated == null) return NotFound();

                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("Parcele");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("Naziv", ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("Naziv", "Već postoji parcela sa ovim nazivom.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju.");
            }

            ViewBag.KatastarskeOpstine = new SelectList(
                await _katastarskaOpstinaService.GetAll(), "Id", "Naziv", dto.IdKatastarskaOpstina);
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
            var parcela = await _parcelaService.GetById(id);
            if (parcela == null) return NotFound();

            if (parcela.IdKorisnik != idKorisnik) return Unauthorized();

            // 2. Brisanje
            try
            {
                await _parcelaService.DeleteById(id);
                TempData["SuccessMessage"] = "Parcela je obrisana.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Greška pri brisanju parcele.";
            }

            return RedirectToAction("Parcele");
        }
    }
}