using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MojAtar.Core.ServiceContracts;
using Rotativa.AspNetCore;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("izvestaji")]
    public class IzvestajiController : Controller
    {
        private readonly IIzvestajService _izvestajiService;
        private readonly IParcelaService _parcelaService;

        public IzvestajiController(IIzvestajService izvestajiService, IParcelaService parcelaService)
        {
            _izvestajiService = izvestajiService;
            _parcelaService = parcelaService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Izvestaj()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var parcele = await _parcelaService.GetAllForUser(Guid.Parse(userId));
            ViewBag.Parcele = new SelectList(parcele, "Id", "Naziv");

            return View();
        }

        [HttpPost("generisi")]
        public async Task<IActionResult> Generisi(string parcelaId, DateTime? odDatuma, DateTime? doDatuma)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            bool sveParcele = parcelaId == "sve-parcele";
            Guid? parcelaGuid = sveParcele ? null : Guid.Parse(parcelaId);

            var izvestaj = await _izvestajiService.GenerisiIzvestaj(
                Guid.Parse(userId), parcelaGuid, odDatuma, doDatuma, sveParcele);

            // Proveri da li ima uopšte podataka
            if (izvestaj == null ||
                izvestaj.Parcele == null ||
                !izvestaj.Parcele.Any(p =>
                    (p.Radnje != null && p.Radnje.Any())))
            {
                TempData["ErrorMessage"] = "Nema dostupnih podataka za prikaz u izabranom periodu.";
                return RedirectToAction("Izvestaj");
            }

            // Ako se radi o svim parcelama — filtriraj samo one koje imaju sadržaj
            if (sveParcele)
            {
                izvestaj.Parcele = izvestaj.Parcele
                    .Where(p => p.Radnje != null && p.Radnje.Any())
                    .ToList();
            }

            ViewBag.SveParcele = sveParcele;

            return View("Prikaz", izvestaj);
        }


        [HttpPost("pdf")]
        public async Task<IActionResult> Pdf(string parcelaId, DateTime? odDatuma, DateTime? doDatuma)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            bool sveParcele = parcelaId == "sve-parcele";
            Guid? parcelaGuid = sveParcele ? null : Guid.Parse(parcelaId);

            var izvestaj = await _izvestajiService.GenerisiIzvestaj(
                Guid.Parse(userId), parcelaGuid, odDatuma, doDatuma, sveParcele);

            // Ako nema podataka uopšte — ne generiši PDF
            if (izvestaj == null ||
                izvestaj.Parcele == null ||
                !izvestaj.Parcele.Any(p =>
                    (p.Radnje != null && p.Radnje.Any())))
            {
                TempData["ErrorMessage"] = "Nema dostupnih podataka za PDF izveštaj u izabranom periodu.";
                return RedirectToAction("Izvestaj");
            }

            // Filtriraj prazne parcele iz PDF-a
            izvestaj.Parcele = izvestaj.Parcele
                .Where(p =>
                    (p.Radnje != null && p.Radnje.Any()))
                .ToList();

            ViewBag.SveParcele = sveParcele;

            return new ViewAsPdf("PdfIzvestaj", izvestaj)
            {
                FileName = "izvestaj.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }

    }
}
