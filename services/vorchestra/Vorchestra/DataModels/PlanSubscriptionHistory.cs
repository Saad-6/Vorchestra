using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class PlanSubscriptionHistory : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid TenantProjectId { get; set; }
    public Guid PlanId { get; set; }
    public bool IsFreeTrial { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
}
