using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MojAtar.UI.Controllers
{
    [Route("")]

    public class AuthController : Controller
    {
        private readonly IKorisnikService _korisnikService;
        public AuthController(IKorisnikService korisnikService)
        {
            _korisnikService = korisnikService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(KorisnikRequestDTO korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sva logika (heširanje, provera emaila) je u servisu
                    await _korisnikService.RegisterNewUser(korisnikRequest);
                    TempData["SuccessMessage"] = "Korisnik je uspešno registrovan!";
                    return RedirectToAction("Login");
                }
                catch (ArgumentException ex)
                {
                    // Servis vraća ArgumentException ako email već postoji
                    ModelState.AddModelError(nameof(korisnikRequest.Email), ex.Message);
                }
            }
            return View(korisnikRequest);
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Pocetna", "Pocetna");
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(KorisnikRequestDTO korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                // Kontroler poziva servis da obavi SVE provere
                KorisnikResponseDTO? korisnik = await _korisnikService.Authenticate(
                    korisnikRequest.Email,
                    korisnikRequest.Lozinka);

                if (korisnik != null)
                {
                    // Logika za Claims i Cookie je nužno u UI sloju (zbog HttpContext-a)
                    var claims = _korisnikService.GenerateClaims(korisnik);

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(5)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["SuccessMessage"] = "Успешно сте се пријавили!";
                    return RedirectToAction("Pocetna", "Pocetna");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пријављивање није успело. Проверите унесене податке и покушајте поново.");
                }
            }
            return View(korisnikRequest);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("api/login")]
        public async Task<IActionResult> ApiLogin(KorisnikRequestDTO korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                KorisnikResponseDTO? korisnik = await _korisnikService.Authenticate(
                    korisnikRequest.Email,
                    korisnikRequest.Lozinka);

                if (korisnik != null)
                {
                    // 2. Kreiranje Claims-a i postavljanje Cookie-a (Nužno ostaje ovde zbog HttpContext-a)
                    var claims = _korisnikService.GenerateClaims(korisnik);

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(5)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Vraćanje RedirectToAction za MVC aplikaciju (ako API endpoint koristi Cookie Autentifikaciju)
                    return RedirectToAction("Pocetna", "Pocetna");
                }
                else
                {
                    // Greška: Neispravni podaci
                    ModelState.AddModelError(string.Empty, "Neispravan email ili lozinka.");

                    // Vraćanje 401/400 statusa za API endpoint
                    return Unauthorized(new { message = "Prijava nije uspela. Proverite unete podatke." });
                }
            }
            return View(korisnikRequest);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("api/register")]
        public async Task<IActionResult> ApiRegister(KorisnikRequestDTO korisnikRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                KorisnikResponseDTO noviKorisnik = await _korisnikService.RegisterNewUser(korisnikRequest);
                return CreatedAtAction(nameof(Register), noviKorisnik);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
