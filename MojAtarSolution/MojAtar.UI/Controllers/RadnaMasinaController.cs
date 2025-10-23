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
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            await _radnaMasinaService.Add(dto);
            return RedirectToAction("RadneMasine");
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
            if (!ModelState.IsValid)
                return View("Dodaj", dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            dto.Id = id;
            await _radnaMasinaService.Update(dto.Id,dto);
            return RedirectToAction("RadneMasine");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _radnaMasinaService.DeleteById(id);
            return RedirectToAction("RadneMasine");
        }

    }
}
