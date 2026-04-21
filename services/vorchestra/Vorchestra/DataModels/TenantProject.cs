using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class TenantProject : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ServerId { get; set; }
    public string Status { get; set; } = null!;         // Pending, Provisioning, Running, Suspended, Failed
    public bool IsSetupComplete { get; set; }
    public DateTimeOffset? OnboardedAt { get; set; }
    public DateTimeOffset? SuspendedAt { get; set; }
    public string SuspensionReason { get; set; } = string.Empty;
}