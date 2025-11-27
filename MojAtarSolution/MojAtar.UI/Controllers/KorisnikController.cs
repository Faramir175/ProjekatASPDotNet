using Azure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("korisnici")]
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _korisnikService;

        public KorisnikController(IKorisnikService korisnikService, ILogger<KorisnikController> logger)
        {
            _korisnikService = korisnikService;
        }

        //// GET: api/korisnici/svi
        [HttpGet("svi")]
        public async Task<ActionResult<List<KorisnikResponseDTO>>> GetAll()
        {
            List<KorisnikResponseDTO?> korisnici = await _korisnikService.GetAll();
            return Ok(korisnici);
        }

        //// GET: api/korisnici/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KorisnikResponseDTO>> GetById([FromRoute] Guid id)
        {
            //_logger.LogInformation("Fetching user with ID: {id}", id);

            try
            {
                KorisnikResponseDTO? korisnik = await _korisnikService.GetById(id);
                if (korisnik == null)
                {
                    return NotFound($"Person with id {id} does not exist.");
                }

                return Ok(korisnik);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/korisnici
        [HttpPost("post")]
        public async Task<ActionResult<KorisnikResponseDTO>> Add([FromBody] KorisnikRequestDTO korisnikRequest)
        {
            try
            {
                KorisnikResponseDTO? korisnik = await _korisnikService.Add(korisnikRequest);
                return Ok(korisnik);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/korisnici/{id}
        [HttpPut("put/{id}")]
        public async Task<ActionResult<KorisnikResponseDTO>> Update([FromRoute] Guid id, [FromBody] KorisnikRequestDTO korisnikRequest)
        {
            try
            {
                KorisnikResponseDTO? updatedKorisnik = await _korisnikService.Update(id, korisnikRequest);
                return Ok(updatedKorisnik);
            }
            catch (ArgumentException ex) when (ex.Message.Contains("ne postoji"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/korisnici/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete([FromRoute]Guid id)
        {
            try
            {
                bool result = await _korisnikService.DeleteById(id);
                if (!result) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("podesavanja")]
        public async Task<IActionResult> Podesavanja()
        {
            KorisnikResponseDTO? korisnik = new KorisnikResponseDTO
            {
                Email = User.FindFirst(ClaimTypes.Name)?.Value,
            };
            korisnik = await _korisnikService.GetByEmail(korisnik.Email);

            if (korisnik == null)
            {
                return NotFound("Korisnik nije pronađen.");
            }

            var model = new KorisnikRequestDTO
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                TipKorisnika = korisnik.TipKorisnika,
                DatumRegistracije = korisnik.DatumRegistracije
            };
            ViewBag.KorisnikId = korisnik.Id;
            return View(model);
        }
        [HttpPost("azuriraj")]
        public async Task<IActionResult> Azuriraj(KorisnikRequestDTO korisnik)
        {
            // 1. UI logika: dohvati podatke iz forme
            string? trenutnaLozinka = Request.Form["TrenutnaLozinka"];
            Guid korisnikId = Guid.Parse(Request.Form["KorisnikId"]);

            if (string.IsNullOrEmpty(trenutnaLozinka))
            {
                ModelState.AddModelError("TrenutnaLozinka", "Morate uneti trenutnu lozinku.");
                return View(korisnik);
            }

            // 2. KONTROLER POZIVA SERVIS DA OBAVI SVE
            var (updatedKorisnik, newClaims) = await _korisnikService.AzurirajKorisnikaSaVerifikacijom(
                korisnikId,
                trenutnaLozinka,
                korisnik);

            if (updatedKorisnik != null && newClaims != null)
            {

                var identity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(5)
                    });

                TempData["SuccessMessage"] = "Podaci su uspešno ažurirani.";
                return RedirectToAction("Podesavanja");
            }
            else
            {
                // Verifikacija lozinke neuspešna
                ModelState.AddModelError("TrenutnaLozinka", "Trenutna lozinka nije ispravna.");
                return View(korisnik);
            }
        }

        [HttpPost("obrisi")]
        public async Task<ActionResult<bool>> Obrisi()
        {
            try
            {
                string? email = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email)) return Unauthorized();

                bool result = await _korisnikService.DeleteLoggedInUser(email);

                if (!result) return NotFound("Greška prilikom brisanja korisnika.");

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
