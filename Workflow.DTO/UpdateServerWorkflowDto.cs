using Shared.DTO;

namespace Workflow.DTO;

public class UpdateServerWorkflowDto : BaseWorkflowDto
{
    public Guid Id { get; set; }
    public Guid ServerId { get; set; }
}
