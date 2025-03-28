using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("")]

    public class AuthController : Controller
    {
        private readonly IKorisnikService _korisnikService;
        private readonly PasswordHasher<KorisnikRequest> _passwordHasher;
        public AuthController(IKorisnikService korisnikService)
        {
            _korisnikService = korisnikService;
            _passwordHasher = new PasswordHasher<KorisnikRequest>();
        }
        // GET: /register
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /register
        [HttpPost("register")]
        public async Task<IActionResult> Register(KorisnikRequest korisnikRequest)
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
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login( KorisnikRequest korisnikRequest)
        {
            if (ModelState.IsValid)
            {
                // 1. Pronaći korisnika po emailu
                KorisnikResponse? korisnik = await _korisnikService.GetByEmail(korisnikRequest.Email);

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
                    new Claim(ClaimTypes.Role, korisnik.TipKorisnika.ToString()) // Ako koristiš ulogu
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

                        return RedirectToAction("Register", "Auth");

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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}
