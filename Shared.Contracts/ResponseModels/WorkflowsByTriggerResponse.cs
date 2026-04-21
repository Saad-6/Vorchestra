using Shared.DTO;

namespace Shared.Contracts.ResponseModels;

public class WorkflowsByTriggerResponse
{
    public List<WorkflowContextDto> Workflows { get; set; } = new List<WorkflowContextDto>();
}
