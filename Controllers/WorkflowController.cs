using CatelogService.Model.Data;
using Microsoft.AspNetCore.Mvc;

namespace CatelogService.Controllers
{
    [ApiController]
    [Route("api/catalog/workflow")]
    public class WorkflowController : ControllerBase
    {
        private readonly CommandInvoker _commandInvoker = new();

        [HttpGet("")]
        public ActionResult<WorkflowOverviewResponse> Index()
        {
            var availableStates = Enum.GetNames(typeof(OrcState));

            return Ok(new WorkflowOverviewResponse
            {
                AvailableStates = availableStates,
                DefaultState = OrcState.ProductCatalogShow.ToString(),
                RunWorkflowEndpoint = Url?.RouteUrl(
                    routeName: "CatalogWorkflowRun",
                    values: new { state = OrcState.ProductCatalogShow })
            });
        }

        [HttpGet("run/{state?}", Name = "CatalogWorkflowRun")]
        public ActionResult<IReadOnlyList<CommandResult>> Run(string? state = null)
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

        public record WorkflowOverviewResponse
        {
            public IEnumerable<string> AvailableStates { get; init; } = Array.Empty<string>();
            public string DefaultState { get; init; } = string.Empty;
            public string? RunWorkflowEndpoint { get; init; }
            public string Description { get; init; } =
                "Use the run endpoint to execute the catalog orchestration workflow.";
        }
    }
}
