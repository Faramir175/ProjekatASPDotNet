using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.ServiceContracts;

namespace MojAtar.UI.Controllers
{
    [Route("istorija-cena")]
    public class IstorijaCenaController : Controller
    {
        private readonly ICenaResursaService _cenaResursaService;
        private readonly ICenaKultureService _cenaKultureService;

        public IstorijaCenaController(
            ICenaResursaService cenaResursaService,
            ICenaKultureService cenaKultureService)
        {
            _cenaResursaService = cenaResursaService;
            _cenaKultureService = cenaKultureService;
        }

        [HttpGet("resursi")]
        public async Task<IActionResult> Resursi(int skip = 0, int take = 20)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId.Value);

            var cene = await _cenaResursaService.GetPaged(idKorisnik, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.Take = take;
            ViewBag.TotalCount = await _cenaResursaService.GetTotalCount(idKorisnik);
            return View(cene);
        }

        [HttpGet("kulture")]
        public async Task<IActionResult> Kulture(int skip = 0, int take = 20)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            Guid idKorisnik = Guid.Parse(userId.Value);

            var cene = await _cenaKultureService.GetPaged(idKorisnik, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.Take = take;
            ViewBag.TotalCount = await _cenaKultureService.GetTotalCount(idKorisnik);
            return View(cene);
        }
    }
}
