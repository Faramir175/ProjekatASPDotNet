using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("")]
    public class PocetnaController : Controller
    {
        private readonly IPocetnaService _pocetnaService;

        public PocetnaController(IPocetnaService pocetnaService)
        {
            _pocetnaService = pocetnaService;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Pocetna()
        {
            var korisnikId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var ime = User.FindFirst("Ime")?.Value;
            var prezime = User.FindFirst("Prezime")?.Value;
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var uloga = User.FindFirst(ClaimTypes.Role)?.Value;
            var datumRegistracije = DateTime.Parse(User.FindFirst("DatumRegistracije")?.Value);

            var model = new PocetnaViewModel
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                Uloga = uloga,
                DatumRegistracije = datumRegistracije,
                BrojParcela = await _pocetnaService.GetBrojParcelaAsync(korisnikId),
                BrojRadnji = await _pocetnaService.GetBrojRadnjiAsync(korisnikId),
                BrojResursa = await _pocetnaService.GetBrojResursaAsync(korisnikId),
                BrojRadnihMasina = await _pocetnaService.GetBrojRadnihMasinaAsync(korisnikId),
                BrojPrikljucnihMasina = await _pocetnaService.GetBrojPrikljucnihMasinaAsync(korisnikId),
                BrojKultura = await _pocetnaService.GetBrojKulturaAsync(korisnikId),
                PoslednjeRadnje = (await _pocetnaService.GetPoslednjeRadnjeAsync(korisnikId))
    .Select(r => new RadnjaDTO
    {
        Id = r.Id,
        IdParcela = r.IdParcela,
        IdKultura = r.IdKultura,
        DatumIzvrsenja = r.DatumIzvrsenja,
        VremenskiUslovi = r.VremenskiUslovi,
        Napomena = r.Napomena,
        UkupanTrosak = r.UkupanTrosak,
        TipRadnje = r.TipRadnje,
        Prinos = (r is Zetva zetva) ? zetva.Prinos : null,
        Parcela = r.Parcela,
        Kultura = r.Kultura
    })
    .ToList()

        };

            return View(model);
        }
    }

}
