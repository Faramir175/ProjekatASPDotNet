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
            ViewBag.KatastarskeOpstine = new SelectList(await _katastarskaOpstinaService.GetAll(), "Id", "Naziv");
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new ParcelaDTO()); // Prazan model za dodavanje
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(ParcelaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Korisnik nije prijavljen.");
            }
            dto.IdKorisnik = Guid.Parse(userId);
            await _parcelaService.Add(dto);
            return RedirectToAction("Parcele");
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var parcela = await _parcelaService.GetById(id);
            if (parcela == null) return NotFound();

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.KatastarskeOpstine = new SelectList(await _katastarskaOpstinaService.GetAll(), "Id", "Naziv", parcela.IdKatastarskaOpstina);
            return View("Dodaj", parcela);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, ParcelaDTO dto)
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
            await _parcelaService.Update(dto.Id,dto);
            return RedirectToAction("Parcele");
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            await _parcelaService.DeleteById(id);
            return RedirectToAction("Parcele");
        }

    }
}
