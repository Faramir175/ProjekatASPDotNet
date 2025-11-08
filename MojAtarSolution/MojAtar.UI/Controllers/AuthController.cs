using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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
        private readonly PasswordHasher<KorisnikRequestDTO> _passwordHasher;
        public AuthController(IKorisnikService korisnikService)
        {
            _korisnikService = korisnikService;
            _passwordHasher = new PasswordHasher<KorisnikRequestDTO>();
        }
        // GET: /register
        [Authorize(Roles = "Admin")]
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /register
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(KorisnikRequestDTO korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                korisnikRequest.Lozinka = _passwordHasher.HashPassword(korisnikRequest, korisnikRequest.Lozinka);
                await _korisnikService.Add(korisnikRequest);
                return RedirectToAction("Login");
            }
            return View(korisnikRequest);
        }

        // GET: /login
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
        public async Task<IActionResult> Login( KorisnikRequestDTO korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                // 1. Pronaći korisnika po emailu
                KorisnikResponseDTO? korisnik = await _korisnikService.GetByEmail(korisnikRequest.Email);
                //korisnikRequest.Lozinka = _passwordHasher.HashPassword(korisnikRequest, korisnikRequest.Lozinka);
                if (korisnik != null)
                {
                    // 2. Verifikovati lozinku
                    var result = _passwordHasher.VerifyHashedPassword(korisnikRequest, korisnik.Lozinka, korisnikRequest.Lozinka);

                    if (result == PasswordVerificationResult.Success)
                    {
                        // 3. Postaviti autentifikacioni cookie
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),
                        new Claim(ClaimTypes.Name, korisnik.Email),
                        new Claim(ClaimTypes.Role, korisnik.TipKorisnika.ToString()),
                        new Claim("Ime", korisnik.Ime),
                        new Claim("Prezime", korisnik.Prezime),
                        new Claim("DatumRegistracije", korisnik.DatumRegistracije.ToString("o")) // ISO format
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

                        var loggedInUser = HttpContext.User;
                        Console.WriteLine($"Logged in: {loggedInUser.Identity.IsAuthenticated}");
                        Console.WriteLine($"User: {loggedInUser.Identity.Name}");
                        Console.WriteLine($"Role: {loggedInUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value}");
                        Console.WriteLine("User authenticated!");
                        Console.WriteLine($"Is Authenticated: {HttpContext.User.Identity?.IsAuthenticated}");

                        var cookies = HttpContext.Request.Cookies;
                        foreach (var cookie in cookies)
                        {
                            Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
                        }

                        return RedirectToAction("Pocetna", "Pocetna");


                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Пријављивање није успело. Проверите унесене податке и покушајте поново.");
                    }

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
                // 1. Pronaći korisnika po emailu
                KorisnikResponseDTO? korisnik = await _korisnikService.GetByEmail(korisnikRequest.Email);

                if (korisnik != null)
                {
                    // 2. Verifikovati lozinku
                    var result = _passwordHasher.VerifyHashedPassword(korisnikRequest, korisnik.Lozinka, korisnikRequest.Lozinka);

                    if (result == PasswordVerificationResult.Success)
                    {
                        // 3. Postaviti autentifikacioni cookie
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, korisnik.Email),
                        new Claim(ClaimTypes.Role, korisnik.TipKorisnika.ToString()),
                        new Claim("Ime", korisnik.Ime),
                        new Claim("Prezime", korisnik.Prezime),
                        new Claim("DatumRegistracije", korisnik.DatumRegistracije.ToString("o")) // ISO format
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

                        var loggedInUser = HttpContext.User;
                        Console.WriteLine($"Logged in: {loggedInUser.Identity.IsAuthenticated}");
                        Console.WriteLine($"User: {loggedInUser.Identity.Name}");
                        Console.WriteLine($"Role: {loggedInUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value}");
                        Console.WriteLine("User authenticated!");
                        Console.WriteLine($"Is Authenticated: {HttpContext.User.Identity?.IsAuthenticated}");

                        var cookies = HttpContext.Request.Cookies;
                        foreach (var cookie in cookies)
                        {
                            Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
                        }

                        return RedirectToAction("Pocetna", "Pocetna");


                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Neispravna lozinka.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Korisnik sa unetim emailom ne postoji.");
                }
            }
            return View(korisnikRequest);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("api/register")]
        public async Task<IActionResult> ApiRegister(KorisnikRequestDTO korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                korisnikRequest.Lozinka = _passwordHasher.HashPassword(korisnikRequest, korisnikRequest.Lozinka);
                await _korisnikService.Add(korisnikRequest);
                return RedirectToAction("Login");
            }
            return View(korisnikRequest);
        }
    }
}
