using CatelogService.Model.Data;
using Microsoft.AspNetCore.Mvc;
using OrchestrationDemo.ViewModels;

namespace OrchestrationDemo.Controllers
{
    public class WorkflowController : Controller
    {
        private readonly CommandInvoker _commandInvoker = new();

        [HttpGet]
        public IActionResult Index()
        {
            var model = new WorkflowViewModel
            {
                AvailableStates = Enum.GetValues<OrcState>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Run(WorkflowViewModel model)
        {
            var results = _commandInvoker.RunWorkflow(model.SelectedState);
            model.Steps = results;
            model.AvailableStates = Enum.GetValues<OrcState>();
            return View("Index", model);
        }
    }
}
