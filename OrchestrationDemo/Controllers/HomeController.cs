using Microsoft.AspNetCore.Mvc;

namespace OrchestrationDemo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
