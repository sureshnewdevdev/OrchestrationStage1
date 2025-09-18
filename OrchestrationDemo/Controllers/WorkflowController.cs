using CatelogService.Model.Data;
using Microsoft.AspNetCore.Mvc;
using OrchestrationDemo.ViewModels;

namespace OrchestrationDemo.Controllers
{
    [Route("orchestration-demo/workflow")]
    public class WorkflowController : Controller
    {
        private const string IndexViewName = "Index";
        private readonly CommandInvoker _commandInvoker = new();

        [HttpGet("")]
        public IActionResult Index()
        {
            var model = new WorkflowViewModel
            {
                AvailableStates = Enum.GetValues<OrcState>()
            };
            return View(IndexViewName, model);
        }

        [HttpPost("run")]
        [ValidateAntiForgeryToken]
        public IActionResult Run(WorkflowViewModel model)
        {
            var results = _commandInvoker.RunWorkflow(model.SelectedState);
            model.Steps = results;
            model.AvailableStates = Enum.GetValues<OrcState>();
            return View(IndexViewName, model);
        }
    }
}
