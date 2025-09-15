using CatelogService.Model;
using CatelogService.Model.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatelogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public static OrcState _OrcState = OrcState.ProductCatalogShow  ;
        public HomeController()
        {
            _OrcState = OrcState.ProductCatalogShow;
        }

        [HttpGet]
        public IActionResult Index()
        {
            

            CommandInvoker commandInvoker = new CommandInvoker();
            var command = commandInvoker.ActionInvoke(OrcState.ProductCatalogShow);
            command.Execute();
            var nextCommand = command.NextCommand;



            return Ok("Catelog Service is up and running");



        }

    }
}
