using Microsoft.AspNetCore.Mvc;

namespace MojAtar.UI.Controllers
{
    public class ParcelaController : Controller
    {
        [HttpGet("parcele")]
        public IActionResult Dodaj()
        {
            return View();
        }
    }
}
