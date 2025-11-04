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

            bool sveParcele = parcelaId == "sve-parcele"; // 👈 Ako je izabrana opcija "-- Sve parcele --"
            Guid? parcelaGuid = sveParcele ? null : Guid.Parse(parcelaId);

            var izvestaj = await _izvestajiService.GenerisiIzvestaj(
                Guid.Parse(userId), parcelaGuid, odDatuma, doDatuma, sveParcele);

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
