using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("")]
    public class PocetnaController : Controller
    {
        // GET: /
        [Authorize]
        [HttpGet("")]
        public IActionResult Pocetna()
        {

                // Kreiraj instancu svoje klase na osnovu podataka iz claim-ova.
                var korisnik = new KorisnikResponse
                {
                    Ime = User.FindFirst("Ime")?.Value,
                    Prezime = User.FindFirst("Prezime")?.Value,
                    Email = User.FindFirst(ClaimTypes.Name)?.Value,
                    TipKorisnika = (KorisnikTip)Enum.Parse(typeof(KorisnikTip), User.FindFirst(ClaimTypes.Role)?.Value),
                    DatumRegistracije = DateTime.Parse(User.FindFirst("DatumRegistracije")?.Value)
                };

                return View(korisnik);
            


        }
    }
}
