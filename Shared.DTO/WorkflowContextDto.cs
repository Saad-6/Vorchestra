namespace Shared.DTO;

public class WorkflowContextDto : BaseWorkflowDto
{
    public List<Guid> GroupIds { get; set; } = new();
}
