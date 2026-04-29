namespace Shared.Contracts.RequestModels;

public class ServerWorkflowsByTriggerRequest
{
    public string Trigger { get; set; } = string.Empty;
    public Guid ServerId { get; set; }
}
