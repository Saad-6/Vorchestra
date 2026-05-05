using Shared.DTO;

namespace Workflow.DTO;

public class WorkflowDto : BaseWorkflowDto
{
    public Guid Id { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? ServerId { get; set; }
    public List<WorkflowGroupDto> Groups { get; set; } = [];
}
