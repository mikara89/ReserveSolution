using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Contollers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
