using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;

namespace MojAtar.UI.Controllers
{
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
        public async Task<ActionResult<KorisnikResponse>> GetById([FromQuery] Guid id)
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
        [HttpPost("")]
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
        [HttpPut("{id}")]
        public async Task<ActionResult<KorisnikResponse>> Update([FromQuery]Guid id, [FromBody] KorisnikRequest korisnikRequest)
        {
            try
            {
                KorisnikResponse? korisnik = await _korisnikService.Update(id, korisnikRequest);
                if (korisnik == null) return NotFound();
                return Ok(korisnik);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/korisnici/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete([FromQuery]Guid id)
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
