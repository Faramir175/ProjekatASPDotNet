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

            // 1. Priprema parametara (UI logika)
            bool sveParcele = parcelaId == "sve-parcele";
            Guid? parcelaGuid = sveParcele ? null : Guid.Parse(parcelaId);

            try
            {
                // 2. Poziv servisa (Servis vraća sve spremno)
                var izvestaj = await _izvestajiService.GenerisiIzvestaj(
                    Guid.Parse(userId), parcelaGuid, odDatuma, doDatuma, sveParcele);

                ViewBag.SveParcele = sveParcele;
                return View("Prikaz", izvestaj);
            }
            catch (ArgumentException ex)
            {
                // 3. Rukovanje situacijom kad nema podataka
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Izvestaj");
            }
        }

        [HttpPost("pdf")]
        public async Task<IActionResult> Pdf(string parcelaId, DateTime? odDatuma, DateTime? doDatuma)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            bool sveParcele = parcelaId == "sve-parcele";
            Guid? parcelaGuid = sveParcele ? null : Guid.Parse(parcelaId);

            try
            {
                var izvestaj = await _izvestajiService.GenerisiIzvestaj(
                    Guid.Parse(userId), parcelaGuid, odDatuma, doDatuma, sveParcele);

                ViewBag.SveParcele = sveParcele;

                return new ViewAsPdf("PdfIzvestaj", izvestaj)
                {
                    FileName = "izvestaj.pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    CustomSwitches = "--disable-smart-shrinking"
                };
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Nema dostupnih podataka za PDF izveštaj u izabranom periodu.";
                return RedirectToAction("Izvestaj");
            }
        }
    }
}