using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class TenantSubscription : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid TenantProjectId { get; set; }   // subscription is per project instance
    public Guid PlanId { get; set; }
    public string BillingCycle { get; set; } = null!;
    public bool IsFreeTrial { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string Status { get; set; } = null!;           // Active, Expired, Cancelled, PastDue
}
