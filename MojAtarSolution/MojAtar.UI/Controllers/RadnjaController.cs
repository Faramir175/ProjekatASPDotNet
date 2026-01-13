using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MojAtar.Core.Domain;
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
            var parcela = await _parcelaService.GetById(idParcela);
            ViewBag.NazivParcele = parcela?.Naziv ?? "Nepoznata parcela";
            return View(radnje);
        }

        [HttpGet("dodaj")]
        public async Task<IActionResult> Dodaj(Guid? fromParcelaId)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            Guid userId = Guid.Parse(userIdStr);

            await UcitajViewBagove(userId);

            // Resursi i cene
            var resursi = await _resursService.GetAllForUser(userId);
            var ceneResursa = resursi.ToDictionary(r => r.Id.ToString(), r => r.AktuelnaCena);
            ViewBag.CeneResursa = ceneResursa;

            // Ako dolazimo sa određene parcele, prosleđujemo ID da bi View mogao eventualno da je automatski selektuje (opciono)
            ViewBag.FromParcelaId = fromParcelaId;

            // NAPOMENA: Ovde više NE učitavamo "SlobodnaPovrsina" za prvu parcelu
            // jer korisnik sada bira parcele dinamički preko checkbox-ova.

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
                // BITNO: Ovde se poziva servis. Servis treba da pročita dto.Parcele listu
                // i da za svaku parcelu, ako je Setva, kreira ParcelaKultura zapis.
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

            // Servis vraća popunjen DTO sa listom Parcela
            var radnja = await _radnjaService.GetForEdit(id, userId);
            if (radnja == null) return NotFound();

            // Provera vlasništva - prolazimo kroz parcele da vidimo da li pripadaju useru
            if (radnja.Parcele != null && radnja.Parcele.Any())
            {
                // Uzimamo prvu parcelu kao referencu za proveru vlasništva
                var idPrveParcele = radnja.Parcele.First().IdParcela;
                var parcela = await _parcelaService.GetById(idPrveParcele);

                if (parcela == null || parcela.IdKorisnik != userId)
                    return Unauthorized();
            }

            await UcitajViewBagove(userId);

            // Cene resursa na datum izvršenja (za preračun troškova u istoriji)
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
                // BITNO: Servis ovde treba da prođe kroz dto.Parcele.
                // Pošto je ovo Izmena, verovatno samo ažuriraš površinu u ParcelaKultura tabeli 
                // jer parcele ne mogu da se menjaju (disabled checkbox).
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

        // AJAX Metoda za JS
        [HttpGet("slobodnaPovrsina/{idParcela}")]
        public async Task<IActionResult> GetSlobodnaPovrsina(Guid idParcela)
        {
            // Servis računa koliko je slobodno na toj parceli
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