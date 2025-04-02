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
        private readonly PasswordHasher<KorisnikRequestDTO> _passwordHasher;

        //private readonly ILogger<KorisnikController> _logger;

        public KorisnikController(IKorisnikService korisnikService, ILogger<KorisnikController> logger)
        {
            _korisnikService = korisnikService;
            _passwordHasher = new PasswordHasher<KorisnikRequestDTO>();
            //_logger = logger;
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
                korisnikRequest.Lozinka = _passwordHasher.HashPassword(korisnikRequest, korisnikRequest.Lozinka);
                KorisnikResponseDTO? korisnik = await _korisnikService.Add(korisnikRequest);
                return Ok(korisnik);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/korisnici/{id}
        [HttpPut("put/{id}")]
        public async Task<ActionResult<KorisnikResponseDTO>> Update([FromRoute]Guid id, [FromBody] KorisnikRequestDTO korisnikRequest)
        {
            try
            {
                // Uzima se korisnik koji se menja iz baze
                KorisnikResponseDTO? originalniKorisnik = await _korisnikService.GetById(id);
                if (originalniKorisnik == null) return NotFound($"Korisnik sa ID-em {id} ne postoji.");

                // Atributi koji nisu prosledjeni upitom se popunjavaju
                if (string.IsNullOrEmpty(korisnikRequest.Ime)) korisnikRequest.Ime = originalniKorisnik.Ime;
                if (string.IsNullOrEmpty(korisnikRequest.Prezime)) korisnikRequest.Prezime = originalniKorisnik.Prezime;
                if (string.IsNullOrEmpty(korisnikRequest.Email)) korisnikRequest.Email = originalniKorisnik.Email;
                if (!korisnikRequest.TipKorisnika.HasValue) korisnikRequest.TipKorisnika = originalniKorisnik.TipKorisnika;
                if (!korisnikRequest.DatumRegistracije.HasValue) korisnikRequest.DatumRegistracije = originalniKorisnik.DatumRegistracije;
                if (string.IsNullOrEmpty(korisnikRequest.Lozinka)) korisnikRequest.Lozinka = originalniKorisnik.Lozinka;
                if (korisnikRequest.Parcele == null) korisnikRequest.Parcele = originalniKorisnik.Parcele;

                // Sačuvaj ažuriranog korisnika
                KorisnikResponseDTO? updatedKorisnik = await _korisnikService.Update(id, korisnikRequest);
                return Ok(updatedKorisnik);
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
            string? trenutnaLozinka = Request.Form["TrenutnaLozinka"];

            Guid korisnikId = Guid.Parse(Request.Form["KorisnikId"]);


            KorisnikResponseDTO? logovaniKorisnik = new KorisnikResponseDTO
            {
                Email = User.FindFirst(ClaimTypes.Name)?.Value,
            };
            logovaniKorisnik = await _korisnikService.GetByEmail(logovaniKorisnik.Email);

            var result = _passwordHasher.VerifyHashedPassword(null, logovaniKorisnik.Lozinka, trenutnaLozinka);

            if (result== PasswordVerificationResult.Success)
            {
                try
                {
                    // Uzima se korisnik koji se menja iz baze
                    KorisnikResponseDTO? originalniKorisnik = await _korisnikService.GetById(korisnikId);
                    if (originalniKorisnik == null)
                    {
                        ModelState.AddModelError("", $"Korisnik sa ID-em {korisnikId} ne postoji.");
                        return View(korisnik);
                    }

                    // Atributi koji nisu prosledjeni upitom se popunjavaju
                    if (string.IsNullOrEmpty(korisnik.Ime)) korisnik.Ime = originalniKorisnik.Ime;
                    if (string.IsNullOrEmpty(korisnik.Prezime)) korisnik.Prezime = originalniKorisnik.Prezime;
                    if (string.IsNullOrEmpty(korisnik.Email)) korisnik.Email = originalniKorisnik.Email; 
                    if (!korisnik.TipKorisnika.HasValue) korisnik.TipKorisnika = originalniKorisnik.TipKorisnika;
                    if (!korisnik.DatumRegistracije.HasValue) korisnik.DatumRegistracije = originalniKorisnik.DatumRegistracije;
                    if (string.IsNullOrEmpty(korisnik.Lozinka)) korisnik.Lozinka = originalniKorisnik.Lozinka;
                    if (korisnik.Parcele == null) korisnik.Parcele = originalniKorisnik.Parcele;

                    korisnik.Lozinka = _passwordHasher.HashPassword(korisnik, korisnik.Lozinka);

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, korisnik.Email),
                            new Claim(ClaimTypes.Role, korisnik.TipKorisnika.ToString()),
                            new Claim("Ime", korisnik.Ime),
                            new Claim("Prezime", korisnik.Prezime),
                            new Claim("DatumRegistracije", korisnik.DatumRegistracije?.ToString("o")) // ISO format
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Omogućava trajni cookie
                        ExpiresUtc = DateTime.UtcNow.AddHours(5) // Cookie ističe nakon 5 sati
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                    // Sačuvaj ažuriranog korisnika
                    KorisnikResponseDTO? updatedKorisnik = await _korisnikService.Update(korisnikId, korisnik);
                    TempData["SuccessMessage"] = "Podaci su uspešno ažurirani.";
                    return RedirectToAction("Podesavanja");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Greška: {ex.Message}");
                    return View(korisnik);
                }
            }
            return RedirectToAction("Podesavanja");
        }

        [HttpPost("obrisi")]
        public async Task<ActionResult<bool>> Obrisi()
        {
            try
            {
                KorisnikResponseDTO korisnik = new KorisnikResponseDTO
                {
                    Email = User.FindFirst(ClaimTypes.Name)?.Value,
                };
                korisnik = await _korisnikService.GetByEmail(korisnik.Email);
                bool result = await _korisnikService.DeleteById(korisnik.Id);

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
