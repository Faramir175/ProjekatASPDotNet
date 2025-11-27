using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("")]
    public class PocetnaController : Controller
    {
        private readonly IPocetnaService _pocetnaService;

        // IParcelaKulturaService nam više ne treba ovde!
        public PocetnaController(IPocetnaService pocetnaService)
        {
            _pocetnaService = pocetnaService;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Pocetna()
        {
            // 1. Podaci o korisniku (iz Claims/Cookie-a)
            var korisnikIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(korisnikIdStr)) return Unauthorized();

            var korisnikId = Guid.Parse(korisnikIdStr);

            // 2. Podaci iz baze (JEDAN poziv servisu)
            PocetnaDTO model = await _pocetnaService.GetDashboardDataAsync(korisnikId);

            // 3. Dopuna modela podacima o korisniku (UI deo)
            model.Ime = User.FindFirst("Ime")?.Value;
            model.Prezime = User.FindFirst("Prezime")?.Value;
            model.Email = User.FindFirst(ClaimTypes.Name)?.Value;
            model.Uloga = User.FindFirst(ClaimTypes.Role)?.Value;

            var datumRegStr = User.FindFirst("DatumRegistracije")?.Value;
            if (DateTime.TryParse(datumRegStr, out DateTime datumReg))
            {
                model.DatumRegistracije = datumReg;
            }

            return View(model);
        }
    }
}