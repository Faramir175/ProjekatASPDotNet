using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.Domain;
using MojAtar.Core.Domain.Enums;
using MojAtar.Core.DTO;
using MojAtar.Core.DTO.Extensions;
using MojAtar.Core.ServiceContracts;
using System.Security.Claims;

namespace MojAtar.UI.Controllers
{
    [Route("")]
    public class PocetnaController : Controller
    {
        private readonly IPocetnaService _pocetnaService;
        private readonly IParcelaKulturaService _parcelaKulturaService;

        public PocetnaController(IPocetnaService pocetnaService, IParcelaKulturaService parcelaKulturaService)
        {
            _pocetnaService = pocetnaService;
            _parcelaKulturaService = parcelaKulturaService;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Pocetna()
        {
            var korisnikId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var ime = User.FindFirst("Ime")?.Value;
            var prezime = User.FindFirst("Prezime")?.Value;
            var email = User.FindFirst(ClaimTypes.Name)?.Value;
            var uloga = User.FindFirst(ClaimTypes.Role)?.Value;
            var datumRegistracije = DateTime.Parse(User.FindFirst("DatumRegistracije")?.Value);

            var poslednjeRadnje = await _pocetnaService.GetPoslednjeRadnjeAsync(korisnikId);
            var radnjaDTOs = new List<RadnjaDTO>();
            foreach (var r in poslednjeRadnje)
            {
                double? povrsinaZaSetvu = null;

                // Provera za Setva radnje i asinhrono učitavanje Povrsina
                if (r.TipRadnje == RadnjaTip.Setva && r.Id.HasValue)
                {
                    // Koristimo injektovani servis
                    var parcelaKultura = await _parcelaKulturaService.GetBySetvaRadnjaId(r.Id.Value);

                    if (parcelaKultura != null)
                    {
                        povrsinaZaSetvu = (double?)parcelaKultura.Povrsina;
                    }
                }

                // Koristimo vašu extension metodu za mapiranje, prosleđujući Povrsina
                radnjaDTOs.Add(RadnjaExtension.ToRadnjaDTO(r, povrsina: (decimal?)povrsinaZaSetvu));
            
        }

            var model = new PocetnaDTO
                {
                    Ime = ime,
                    Prezime = prezime,
                    Email = email,
                    Uloga = uloga,
                    DatumRegistracije = datumRegistracije,
                    BrojParcela = await _pocetnaService.GetBrojParcelaAsync(korisnikId),
                    BrojRadnji = await _pocetnaService.GetBrojRadnjiAsync(korisnikId),
                    BrojResursa = await _pocetnaService.GetBrojResursaAsync(korisnikId),
                    BrojRadnihMasina = await _pocetnaService.GetBrojRadnihMasinaAsync(korisnikId),
                    BrojPrikljucnihMasina = await _pocetnaService.GetBrojPrikljucnihMasinaAsync(korisnikId),
                    BrojKultura = await _pocetnaService.GetBrojKulturaAsync(korisnikId),
                    PoslednjeRadnje = radnjaDTOs
                };

                return View(model);
            }
        }
    }
