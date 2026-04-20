using Shared.DTO;

namespace Workflow.DTO;

public class CreateServerWorkflowDto : BaseWorkflowDto
{
    public Guid ServerId { get; set; }
}
