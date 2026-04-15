using Shared.Domain.DataModels;

namespace Vbaton.Domain.DataModels;

public class ExecutionLog : BaseEntity
{
    public Guid? ServerId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? GroupId { get; set; }
    public bool Succeeded { get; set; }
    public string Output { get; set; }

}
