namespace Shared.Contracts.RequestModels;

public class ProjectWorkflowsByTriggerRequest
{
    public string Trigger { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}
