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
    [Route("prikljucne-masine")]
    public class PrikljucnaMasinaController : Controller
    {
        private readonly IPrikljucnaMasinaService _prikljucnaMasinaService;

        public PrikljucnaMasinaController(IPrikljucnaMasinaService prikljucnaMasinaService)
        {
            _prikljucnaMasinaService = prikljucnaMasinaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> PrikljucneMasine()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);
            var radneMasine = await _prikljucnaMasinaService.GetAllForUser(idKorisnik);

            return View(radneMasine);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new PrikljucnaMasinaDTO()); // Prazan model za dodavanje
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(PrikljucnaMasinaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            await _prikljucnaMasinaService.Add(dto);
            return RedirectToAction("PrikljucneMasine");
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var prikljucnaMasina = await _prikljucnaMasinaService.GetById(id);
            if (prikljucnaMasina == null) return NotFound();

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View("Dodaj", prikljucnaMasina);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, PrikljucnaMasinaDTO dto)
        {
            if (!ModelState.IsValid)
                return View("Dodaj", dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            dto.Id = id;
            await _prikljucnaMasinaService.Update(dto.Id, dto);
            return RedirectToAction("PrikljucneMasine");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _prikljucnaMasinaService.DeleteById(id);
            return RedirectToAction("PrikljucneMasine");
        }

    }
}
