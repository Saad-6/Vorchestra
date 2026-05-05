namespace Workflow.DTO;

public class WorkflowGroupDto
{
    public Guid GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public int Order { get; set; }
}
