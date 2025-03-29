using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;

namespace MojAtar.UI.Controllers
{
    [Route("korisnici")]
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _korisnikService;
        //private readonly ILogger<KorisnikController> _logger;

        public KorisnikController(IKorisnikService korisnikService, ILogger<KorisnikController> logger)
        {
            _korisnikService = korisnikService;
            //_logger = logger;
        }

        //// GET: api/korisnici/svi
        [HttpGet("svi")]
        public async Task<ActionResult<List<KorisnikResponse>>> GetAll()
        {
            List<KorisnikResponse?> korisnici = await _korisnikService.GetAll();
            return Ok(korisnici);
        }

        //// GET: api/korisnici/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KorisnikResponse>> GetById([FromRoute] Guid id)
        {
            //_logger.LogInformation("Fetching user with ID: {id}", id);

            try
            {
                KorisnikResponse? korisnik = await _korisnikService.GetById(id);
                if (korisnik == null)
                {
                    return NotFound($"Person with id {id} does not exist.");
                }

                return Ok(korisnik);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //POST: api/korisnici
        [HttpPost("post")]
        public async Task<ActionResult<KorisnikResponse>> Add([FromBody] KorisnikRequest korisnikRequest)
        {
            try
            {
                KorisnikResponse? korisnik = await _korisnikService.Add(korisnikRequest);
                return Ok(korisnik);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/korisnici/{id}
        [HttpPut("put/{id}")]
        public async Task<ActionResult<KorisnikResponse>> Update([FromRoute]Guid id, [FromBody] KorisnikRequest korisnikRequest)
        {
            try
            {
                // Uzima se korisnik koji se menja iz baze
                KorisnikResponse? originalniKorisnik = await _korisnikService.GetById(id);
                if (originalniKorisnik == null) return NotFound($"Korisnik sa ID-em {id} ne postoji.");

                // Atributi koji nisu prosledjeni upitom se popunjavaju
                if (string.IsNullOrEmpty(korisnikRequest.Ime)) korisnikRequest.Ime = originalniKorisnik.Ime;
                if (string.IsNullOrEmpty(korisnikRequest.Prezime)) korisnikRequest.Prezime = originalniKorisnik.Prezime;
                if (string.IsNullOrEmpty(korisnikRequest.Email)) korisnikRequest.Email = originalniKorisnik.Email;
                if (!korisnikRequest.TipKorisnika.HasValue) korisnikRequest.TipKorisnika = originalniKorisnik.TipKorisnika;
                if (!korisnikRequest.DatumRegistracije.HasValue) korisnikRequest.DatumRegistracije = originalniKorisnik.DatumRegistracije;
                if (string.IsNullOrEmpty(korisnikRequest.Lozinka)) korisnikRequest.Lozinka = originalniKorisnik.Lozinka;
                if (korisnikRequest.Parcele == null) korisnikRequest.Parcele = originalniKorisnik.Parcele;

                // Sačuvaj ažuriranog korisnika
                KorisnikResponse? updatedKorisnik = await _korisnikService.Update(id, korisnikRequest);
                return Ok(updatedKorisnik);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/korisnici/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<bool>> Delete([FromRoute]Guid id)
        {
            try
            {
                bool result = await _korisnikService.DeleteById(id);
                if (!result) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
