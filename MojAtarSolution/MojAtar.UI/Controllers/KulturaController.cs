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
        public async Task<IActionResult> Kulture()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);
            var kulture = await _kulturaService.GetAllForUser(idKorisnik);

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
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            await _kulturaService.Add(dto);
            return RedirectToAction("Kulture");
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var kultura = await _kulturaService.GetById(id);
            if (kultura == null) return NotFound();

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View("Dodaj", kultura);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, KulturaDTO dto)
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
            await _kulturaService.Update(dto.Id,dto);
            return RedirectToAction("Kulture");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _kulturaService.DeleteById(id);
            return RedirectToAction("Kulture");
        }

    }
}
