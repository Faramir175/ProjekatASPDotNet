using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.Domain;
using MojAtar.Core.ServiceContracts;

namespace MojAtar.UI.Controllers
{
    [Route("istorija-cena")]
    public class IstorijaCenaController : Controller
    {
        private readonly ICenaResursaService _cenaResursaService;
        private readonly ICenaKultureService _cenaKultureService;
        private readonly IKorisnikService _korisnikService;

        public IstorijaCenaController(
            ICenaResursaService cenaResursaService,
            ICenaKultureService cenaKultureService,
            IKorisnikService korisnikService)
        {
            _cenaResursaService = cenaResursaService;
            _cenaKultureService = cenaKultureService;
            _korisnikService = korisnikService;
        }

        [HttpGet("resursi")]
        public async Task<IActionResult> Resursi(int skip = 0, int take = 20)
        {
            Guid? idKorisnik = _korisnikService.GetKorisnikIdFromClaims(User);
            if (idKorisnik == null) return Unauthorized();

            var cene = await _cenaResursaService.GetPaged(idKorisnik.Value, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.Take = take;
            ViewBag.TotalCount = await _cenaResursaService.GetTotalCount(idKorisnik.Value);
            return View(cene);
        }

        [HttpGet("kulture")]
        public async Task<IActionResult> Kulture(int skip = 0, int take = 20)
        {
            try
            {
                Guid? idKorisnik = _korisnikService.GetKorisnikIdFromClaims(User);
                if (idKorisnik == null) return Unauthorized();

                var cene = await _cenaKultureService.GetPaged(idKorisnik.Value, skip, take);

                ViewBag.Skip = skip + take;
                ViewBag.Take = take;
                ViewBag.TotalCount = await _cenaKultureService.GetTotalCount(idKorisnik.Value);

                return View(cene);
            }
            catch
            {
                TempData["ErrorMessage"] = "Грешка у претрази историје цена културе.";
                return View(new List<CenaKulture>());
            }
        }

    }
}
