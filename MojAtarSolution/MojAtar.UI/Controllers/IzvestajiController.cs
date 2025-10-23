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
        public async Task<IActionResult> Generisi(Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var izvestaj = await _izvestajiService.GenerisiIzvestaj(Guid.Parse(userId), parcelaId, odDatuma, doDatuma, sveParcele);
            return View("Prikaz", izvestaj);
        }

        [HttpPost("pdf")]
        public async Task<IActionResult> Pdf(Guid? parcelaId, DateTime? odDatuma, DateTime? doDatuma, bool sveParcele)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var izvestaj = await _izvestajiService.GenerisiIzvestaj(Guid.Parse(userId), parcelaId, odDatuma, doDatuma, sveParcele);

            return new ViewAsPdf("PdfIzvestaj", izvestaj) // 👈 novi view bez layouta
            {
                FileName = "izvestaj.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
    }
}
