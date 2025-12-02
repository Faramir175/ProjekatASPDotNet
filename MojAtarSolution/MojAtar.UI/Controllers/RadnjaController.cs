using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("radnje")]
    public class RadnjaController : Controller
    {
        private readonly IRadnjaService _radnjaService;
        private readonly IKulturaService _kulturaService;
        private readonly IParcelaService _parcelaService;
        private readonly IRadnaMasinaService _radnaMasinaService;
        private readonly IPrikljucnaMasinaService _prikljucnaMasinaService;
        private readonly IResursService _resursService;
        private readonly ICenaResursaService _cenaResursaService;

        // Manje zavisnosti jer su ostale prešle u servis
        public RadnjaController(
            IRadnjaService radnjaService,
            IKulturaService kulturaService,
            IParcelaService parcelaService,
            IRadnaMasinaService radnaMasinaService,
            IPrikljucnaMasinaService prikljucnaMasinaService,
            IResursService resursService,
            ICenaResursaService cenaResursaService)
        {
            _radnjaService = radnjaService;
            _kulturaService = kulturaService;
            _parcelaService = parcelaService;
            _radnaMasinaService = radnaMasinaService;
            _prikljucnaMasinaService = prikljucnaMasinaService;
            _resursService = resursService;
            _cenaResursaService = cenaResursaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Radnje(int skip = 0, int take = 9)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            // Servis vraća sve spremno (uključujući površinu za setvu ako si je mapirao u servisu)
            var radnje = await _radnjaService.GetAllByKorisnikPaged(userId, skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.TotalCount = await _radnjaService.GetCountByKorisnik(userId);

            return View(radnje);
        }

        [HttpGet("RadnjePoParceli/{idParcela}")]
        public async Task<IActionResult> RadnjePoParceli(Guid idParcela, int skip = 0, int take = 9)
        {
            var radnje = await _radnjaService.GetAllByParcelaPaged(idParcela, skip, take);
            ViewBag.Skip = skip + take;
            ViewBag.IdParcela = idParcela;
            ViewBag.TotalCount = await _radnjaService.GetCountByParcela(idParcela);
            return View(radnje);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            await UcitajViewBagove(userId);

            // Inicijalno slobodna površina za prvu parcelu
            var parcele = await _parcelaService.GetAllForUser(userId);
            if (parcele.Any())
            {
                ViewBag.SlobodnaPovrsina = await _radnjaService.GetSlobodnaPovrsinaAsync(parcele.First().Id!.Value);
            }

            // Cene resursa (ovo može i u posebnu AJAX metodu, ali ok je i ovde)
            var resursi = await _resursService.GetAllForUser(userId);
            var ceneResursa = resursi.ToDictionary(r => r.Id.ToString(), r => r.AktuelnaCena);
            ViewBag.CeneResursa = ceneResursa;

            return View(new RadnjaDTO());
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(RadnjaDTO dto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            if (!ModelState.IsValid)
            {
                await UcitajViewBagove(userId);
                return View(dto);
            }

            try
            {
                // Sva logika je sada u servisu!
                await _radnjaService.Add(dto);
                TempData["SuccessMessage"] = "Radnja je uspešno dodata!";
                return RedirectToAction("Radnje");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await UcitajViewBagove(userId);
                return View(dto);
            }
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            // Servis vraća popunjen DTO sa svim vezama
            var radnja = await _radnjaService.GetForEdit(id, userId);
            if (radnja == null) return NotFound();

            // Sigurnosna provera (da li parcela pripada korisniku?) - može i u servisu, ali ok je i ovde
            var parcela = await _parcelaService.GetById(radnja.IdParcela);
            if (parcela == null || parcela.IdKorisnik != userId) return Unauthorized();

            await UcitajViewBagove(userId);

            // Cene resursa na datum izvršenja
            var resursi = await _resursService.GetAllForUser(userId);
            var ceneResursa = new Dictionary<string, double>();
            foreach (var res in resursi)
            {
                var cena = await _cenaResursaService.GetAktuelnaCena(userId, (Guid)res.Id, radnja.DatumIzvrsenja);
                ceneResursa[res.Id.ToString()] = cena;
            }
            ViewBag.CeneResursa = ceneResursa;

            return View("Dodaj", radnja);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, RadnjaDTO dto, List<Guid>? ObrisaneRadneMasineId, List<Guid>? ObrisanePrikljucneMasineId, List<Guid>? ObrisaniResursiId)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            if (!ModelState.IsValid)
            {
                await UcitajViewBagove(userId);
                return View("Dodaj", dto);
            }

            try
            {
                // Servis prima liste za brisanje i sve odrađuje
                await _radnjaService.Update(id, dto, ObrisaneRadneMasineId, ObrisanePrikljucneMasineId, ObrisaniResursiId);
                TempData["SuccessMessage"] = "Izmene su uspešno sačuvane!";
                return RedirectToAction("Radnje");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                await UcitajViewBagove(userId);
                return View("Dodaj", dto);
            }
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            // Opciono: Provera vlasništva pre brisanja
            try
            {
                await _radnjaService.DeleteById(id);
                TempData["SuccessMessage"] = "Radnja je uspešno obrisana!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Radnje");
        }

        [HttpGet("slobodnaPovrsina/{idParcela}")]
        public async Task<IActionResult> GetSlobodnaPovrsina(Guid idParcela)
        {
            var slobodno = await _radnjaService.GetSlobodnaPovrsinaAsync(idParcela);
            return Json(new { slobodno });
        }

        private async Task UcitajViewBagove(Guid userId)
        {
            ViewBag.KultureSelectList = new SelectList(await _kulturaService.GetAllForUser(userId), "Id", "Naziv");
            ViewBag.ParceleSelectList = new SelectList(await _parcelaService.GetAllForUser(userId), "Id", "Naziv");
            ViewBag.RadneMasineSelectList = new SelectList(await _radnaMasinaService.GetAllForUser(userId), "Id", "Naziv");
            ViewBag.PrikljucneMasineSelectList = new SelectList(await _prikljucnaMasinaService.GetAllForUser(userId), "Id", "Naziv");
            ViewBag.ResursiSelectList = new SelectList(await _resursService.GetAllForUser(userId), "Id", "Naziv");
        }
    }
}