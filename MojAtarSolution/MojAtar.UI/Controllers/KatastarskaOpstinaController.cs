using Microsoft.AspNetCore.Mvc;
using MojAtar.Core.DTO;
using MojAtar.Core.ServiceContracts;
using MojAtar.Infrastructure.MojAtar;

namespace MojAtar.UI.Controllers
{
    [Route("api/katastarske-opstine")]
    [ApiController]
    public class KatastarskaOpstinaController : ControllerBase
    {
        private readonly IKatastarskaOpstinaService _katastarskaOpstinaService;

        public KatastarskaOpstinaController(IKatastarskaOpstinaService katastarskaOpstinaService)
        {
            _katastarskaOpstinaService = katastarskaOpstinaService;
        }

        [Route("svi")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KatastarskaOpstinaDTO>>> GetAll()
        {
            var opstine = await _katastarskaOpstinaService.GetAll();
            return Ok(opstine);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KatastarskaOpstinaDTO>> GetById([FromRoute]Guid id)
        {
            KatastarskaOpstinaDTO opstina = await _katastarskaOpstinaService.GetById(id);
            if (opstina == null)
                return NotFound();

            return Ok(opstina);
        }

        [HttpPost("post")]
        public async Task<ActionResult<KatastarskaOpstinaDTO>> Add([FromBody]KatastarskaOpstinaDTO dto)
        {
            var novaOpstina = await _katastarskaOpstinaService.Add(dto);
            return CreatedAtAction(nameof(GetById), new { id = novaOpstina.Id }, novaOpstina);
        }

        [HttpPost("put/{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] KatastarskaOpstinaDTO dto)
        {
            dto.Id = id;
            var updated = await _katastarskaOpstinaService.Update(dto);
            if (updated==null)
                return NotFound();

            return NoContent();
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deleted = await _katastarskaOpstinaService.DeleteById(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
