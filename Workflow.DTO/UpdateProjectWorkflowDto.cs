namespace Workflow.DTO;

using Shared.DTO;

public class UpdateProjectWorkflowDto : BaseWorkflowDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
}
