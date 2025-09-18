using CatelogService.Model.Data;

namespace CatelogService.ViewModels
{
    public class WorkflowViewModel
    {
        public OrcState SelectedState { get; set; } = OrcState.ProductCatalogShow;
        public IReadOnlyList<CommandResult> Steps { get; set; } = Array.Empty<CommandResult>();
        public IEnumerable<OrcState> AvailableStates { get; set; } = Enum.GetValues<OrcState>();
    }
}
