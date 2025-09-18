using CatelogService.Model.Data;
using Microsoft.AspNetCore.Mvc;

namespace CatelogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrchestrationController : ControllerBase
    {
        private readonly CommandInvoker _commandInvoker = new();

        [HttpGet("states")]
        public ActionResult<IEnumerable<string>> GetStates()
        {
            var states = Enum.GetNames(typeof(OrcState));
            return Ok(states);
        }

        [HttpGet("run/{state?}")]
        public ActionResult<IEnumerable<CommandResult>> Run(string? state = null)
        {
            var targetState = OrcState.ProductCatalogShow;

            if (!string.IsNullOrWhiteSpace(state) &&
                !Enum.TryParse(state, ignoreCase: true, out targetState))
            {
                return BadRequest($"Unknown state '{state}'.");
            }

            var results = _commandInvoker.RunWorkflow(targetState);
            return Ok(results);
        }
    }
}
