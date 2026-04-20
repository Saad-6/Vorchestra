using Shared.Domain.DataModels;

namespace Workflow.Domain.DataModels;

public class WorkflowGroup : BaseEntity
{
    public Guid WorkflowId { get; set; }
    public Guid GroupId { get; set; }
    public int Order { get; set; }
}
