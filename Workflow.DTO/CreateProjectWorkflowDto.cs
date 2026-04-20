using Shared.DTO;

namespace Workflow.DTO;

public class CreateProjectWorkflowDto : BaseWorkflowDto
{
    public Guid ProjectId { get; set; }
}
