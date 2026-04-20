using Shared.Domain.DataModels;

namespace Workflow.Domain.DataModels;

public class ProjectWorkflow : BaseEntity
{
    public Guid ProjectId {  get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
    public string Trigger { get; set; } = null!;
}
