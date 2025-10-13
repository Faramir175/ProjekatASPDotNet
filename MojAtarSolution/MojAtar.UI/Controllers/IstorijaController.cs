using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojAtar.Infrastructure.MojAtar;

namespace MojAtar.UI.Controllers
{
    [Route("istorija-cena")]
    public class IstorijaCenaController : Controller
    {
        private readonly MojAtarDbContext _dbContext;

        public IstorijaCenaController(MojAtarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("resursi")]
        public async Task<IActionResult> Resursi()
        {
            var cene = await _dbContext.CeneResursa
                .Include(c => c.Resurs)
                .OrderByDescending(c => c.DatumVaznosti)
                .ToListAsync();

            return View(cene);
        }

        [HttpGet("kulture")]
        public async Task<IActionResult> Kulture()
        {
            var cene = await _dbContext.CeneKultura
                .Include(c => c.Kultura)
                .OrderByDescending(c => c.DatumVaznosti)
                .ToListAsync();

            return View(cene);
        }
    }
}
