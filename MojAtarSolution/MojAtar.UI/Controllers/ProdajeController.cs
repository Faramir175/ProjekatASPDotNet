using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("prodaje")]
    public class ProdajeController : Controller
    {
        private readonly IProdajaService _prodajaService;
        private readonly IKulturaService _kulturaService;
        private readonly ICenaKultureService _cenaKultureService;

        public ProdajeController(
            IProdajaService prodajaService,
            IKulturaService kulturaService,
            ICenaKultureService cenaKultureService)
        {
            _prodajaService = prodajaService;
            _kulturaService = kulturaService;
            _cenaKultureService = cenaKultureService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Prodaje(int skip = 0, int take = 20)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            Guid userId = Guid.Parse(userIdStr);

            var prodaje = await _prodajaService.GetPaged(userId, skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.Take = take;
            ViewBag.TotalCount = await _prodajaService.GetTotalCount(userId);

            return View(prodaje);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            ViewBag.UserId = userIdStr;

            var kulture = await _kulturaService.GetAllForUser(userId);
            ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");

            double? aktuelnaCena = null;
            if (kulture.Any())
            {
                var prvaKultura = kulture.First();
                aktuelnaCena = await _cenaKultureService.GetAktuelnaCena(userId, prvaKultura.Id!.Value, DateTime.Now);
            }

            ViewBag.AktuelnaCena = aktuelnaCena;

            var model = new ProdajaDTO
            {
                DatumProdaje = DateTime.Now,
                CenaPoJedinici = (decimal?)aktuelnaCena
            };

            return View(model);
        }

        [HttpPost("dodaj")]
        public async Task<IActionResult> Dodaj(ProdajaDTO dto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            if (!ModelState.IsValid)
            {
                var kulture = await _kulturaService.GetAllForUser(userId);
                ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");
                return View(dto);
            }

            try
            {
                // Provera: Da li kultura pripada ovom korisniku?
                var kultura = await _kulturaService.GetById(dto.IdKultura);
                if (kultura == null || kultura.IdKorisnik != userId) return Unauthorized();

                await _prodajaService.Add(dto);
                TempData["SuccessMessage"] = "Prodaja je uspešno dodata!";
                return RedirectToAction("Prodaje");
            }
            catch (Exception ex) // Hvatamo sve greške (InvalidOperation, KeyNotFound...)
            {
                ModelState.AddModelError("", ex.Message); // Prikazujemo poruku korisniku (npr. "Nema dovoljno količine")
            }

            var kulturePonovo = await _kulturaService.GetAllForUser(userId);
            ViewBag.Kulture = new SelectList(kulturePonovo, "Id", "Naziv");
            return View(dto);
        }

        [HttpGet("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            var prodaja = await _prodajaService.GetById(id);
            if (prodaja == null) return NotFound();

            // PROVERA VLASNIŠTVA preko kulture
            var kultura = await _kulturaService.GetById(prodaja.IdKultura);
            if (kultura == null || kultura.IdKorisnik != userId) return Unauthorized();

            var kultureList = await _kulturaService.GetAllForUser(userId);
            ViewBag.Kulture = new SelectList(kultureList, "Id", "Naziv", prodaja.IdKultura);

            return View("Dodaj", prodaja);
        }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, ProdajaDTO dto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            dto.Id = id;

            if (!ModelState.IsValid)
            {
                var kulture = await _kulturaService.GetAllForUser(userId);
                ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");
                return View("Dodaj", dto);
            }

            try
            {
                // Ponovo provera vlasništva pre izmene
                var kultura = await _kulturaService.GetById(dto.IdKultura);
                if (kultura == null || kultura.IdKorisnik != userId) return Unauthorized();

                await _prodajaService.Update(dto);
                TempData["SuccessMessage"] = "Prodaja je uspešno izmenjena!";
                return RedirectToAction("Prodaje");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            var kulturePonovo = await _kulturaService.GetAllForUser(userId);
            ViewBag.Kulture = new SelectList(kulturePonovo, "Id", "Naziv");
            return View("Dodaj", dto);
        }

        [HttpPost("obrisi/{id}")]
        public async Task<IActionResult> Obrisi(Guid id)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            var prodaja = await _prodajaService.GetById(id);
            if (prodaja == null) return NotFound();

            // Provera vlasništva
            var kultura = await _kulturaService.GetById(prodaja.IdKultura);
            if (kultura == null || kultura.IdKorisnik != userId) return Unauthorized();

            await _prodajaService.Delete(id);

            TempData["SuccessMessage"] = "Prodaja je obrisana.";
            return RedirectToAction("Prodaje");
        }

        // API metode za AJAX pozive iz View-a

        [HttpGet("getcena")]
        public async Task<IActionResult> GetCena(Guid idKultura, DateTime datum)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            // Sigurnosna provera: da li korisnik sme da vidi cenu ove kulture?
            var kultura = await _kulturaService.GetById(idKultura);
            if (kultura == null || kultura.IdKorisnik != userId) return Unauthorized();

            double cena = await _cenaKultureService.GetAktuelnaCena(userId, idKultura, datum);
            return Json(new { cena });
        }

        [HttpGet("getraspolozivo")]
        public async Task<IActionResult> GetRaspolozivo(Guid idKultura)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            var kultura = await _kulturaService.GetById(idKultura);
            if (kultura == null || kultura.IdKorisnik != userId) return Unauthorized();

            decimal raspolozivo = kultura.RaspolozivoZaProdaju;
            return Json(new { raspolozivo });
        }
    }
}