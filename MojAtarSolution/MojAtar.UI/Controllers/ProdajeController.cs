using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using MojAtar.Core.Services;
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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var prodaje = await _prodajaService.GetPaged(Guid.Parse(userId), skip, take);

            ViewBag.Skip = skip + take;
            ViewBag.Take = take;
            ViewBag.TotalCount = await _prodajaService.GetTotalCount(Guid.Parse(userId));

            return View(prodaje);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ViewBag.UserId = userId;

            // Sve kulture za korisnika
            var kulture = await _kulturaService.GetAllForUser(Guid.Parse(userId));
            ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");

            // Ako postoji makar jedna kultura, odmah učitaj njenu aktuelnu cenu za današnji datum
            double? aktuelnaCena = null;
            if (kulture.Any())
            {
                var prvaKultura = kulture.First();
                aktuelnaCena = await _cenaKultureService.GetAktuelnaCena(Guid.Parse(userId), prvaKultura.Id!.Value, DateTime.Now);
            }

            // Pošalji vrednost u View
            ViewBag.AktuelnaCena = aktuelnaCena;

            // Napravi novi DTO i postavi današnji datum kao podrazumevani
            var model = new ProdajaDTO
            {
                DatumProdaje = DateTime.Now,
                CenaPoJedinici = (decimal?)aktuelnaCena
            };

            return View(model);
        }

        [HttpGet("getcena")]
        public async Task<IActionResult> GetCena(Guid idKultura, DateTime datum)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Dobij tačnu cenu koja je važila tog dana
            double cena = await _cenaKultureService.GetAktuelnaCena(Guid.Parse(userId), idKultura, datum);

            return Json(new { cena });
        }

        [HttpGet("getraspolozivo")]
        public async Task<IActionResult> GetRaspolozivo(Guid idKultura)
        {
            var kultura = await _kulturaService.GetById(idKultura);
            decimal raspolozivo = kultura?.RaspolozivoZaProdaju ?? 0;
            return Json(new { raspolozivo });
        }


        [HttpPost("dodaj")]
            public async Task<IActionResult> Dodaj(ProdajaDTO dto)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                if (!ModelState.IsValid)
                {
                    var kulture = await _kulturaService.GetAllForUser(Guid.Parse(userId));
                    ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");
                    return View(dto);
                }

                try
                {
                    await _prodajaService.Add(dto);
                    TempData["SuccessMessage"] = "Prodaja je uspešno dodata!";
                    return RedirectToAction("Prodaje");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("Kolicina", ex.Message);
                }

                var kulturePonovo = await _kulturaService.GetAllForUser(Guid.Parse(userId));
                ViewBag.Kulture = new SelectList(kulturePonovo, "Id", "Naziv");
                return View(dto);
            }

            [HttpGet("izmeni/{id}")]
            public async Task<IActionResult> Izmeni(Guid id)
            {
                var prodaja = await _prodajaService.GetById(id);
                if (prodaja == null) return NotFound();

                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var kulture = await _kulturaService.GetAllForUser(Guid.Parse(userId));
                ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");

                return View("Dodaj", prodaja); // koristi isti View kao Dodaj
            }

        [HttpPost("izmeni/{id}")]
        public async Task<IActionResult> Izmeni(Guid id, ProdajaDTO dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            dto.Id = id;

            if (!ModelState.IsValid)
            {
                var kulture = await _kulturaService.GetAllForUser(Guid.Parse(userId));
                ViewBag.Kulture = new SelectList(kulture, "Id", "Naziv");
                return View("Dodaj", dto);
            }

            try
            {
                await _prodajaService.Update(dto);
                TempData["SuccessMessage"] = "Prodaja je uspešno izmenjena!";
                return RedirectToAction("Prodaje");
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Kolicina", ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Greška pri čuvanju promena.");
            }

            var kulturePonovo = await _kulturaService.GetAllForUser(Guid.Parse(userId));
            ViewBag.Kulture = new SelectList(kulturePonovo, "Id", "Naziv");
            return View("Dodaj", dto);
        }

        [HttpPost("obrisi/{id}")]
            public async Task<IActionResult> Obrisi(Guid id)
            {
                await _prodajaService.Delete(id);
                return RedirectToAction("Prodaje");
            }
        }
    }
