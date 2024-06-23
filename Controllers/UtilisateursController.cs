using Microsoft.AspNetCore.Mvc;

namespace LearnHubFO.Controllers
{
    public class UtilisateursController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
