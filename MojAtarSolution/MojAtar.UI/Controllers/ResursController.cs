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
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new ResursDTO()); // Prazan model za dodavanje
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(ResursDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            await _resursService.Add(dto);
            return RedirectToAction("Resursi");
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var resurs = await _resursService.GetById(id);
            if (resurs == null) return NotFound();

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View("Dodaj", resurs);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, ResursDTO dto)
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
            await _resursService.Update(dto.Id,dto);
            return RedirectToAction("Resursi");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _resursService.DeleteById(id);
            return RedirectToAction("Resursi");
        }

    }
}
