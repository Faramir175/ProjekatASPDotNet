﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("radnje")]
    public class RadnjaController : Controller
    {
        private readonly IRadnjaService _radnjaService;

        public RadnjaController(IRadnjaService radnjaService)
        {
            _radnjaService = radnjaService;
        }

        // Prikaz poslednjih 10 radnji korisnika
        [HttpGet("")]
        public async Task<IActionResult> Radnje(int skip = 0, int take = 10)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId);

            var radnje = await _radnjaService.GetAllByKorisnikPaged(idKorisnik, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _radnjaService.GetCountByKorisnik(idKorisnik);

            return View(radnje);
        }


        // Prikaz radnji za konkretnu parcelu
        [HttpGet("RadnjePoParceli/{idParcela}")]
        public async Task<IActionResult> RadnjePoParceli(Guid idParcela, int skip = 0, int take = 10)
        {
            var radnje = await _radnjaService.GetAllByParcelaPaged(idParcela, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.IdParcela = idParcela;
            ViewBag.TotalCount = await _radnjaService.GetCountByParcela(idParcela);

            return View(radnje);
        }


        // Dodavanje nove radnje
        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new RadnjaDTO());
        }


        [HttpPost("dodaj/{idParcela}")]
        public async Task<IActionResult> Dodaj(RadnjaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _radnjaService.Add(dto);
            return RedirectToAction("RadnjePoParceli", new { idParcela = dto.IdParcela });
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            var radnja = await _radnjaService.GetById(id);

            if (radnja == null)
                return NotFound();

            return View(radnja);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnjaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _radnjaService.Update(id, dto);

            return RedirectToAction("RadnjePoParceli", new { idParcela = dto.IdParcela });
        }

    }
}
