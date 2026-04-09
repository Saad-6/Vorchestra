using Vorchestra.Domain.Constants;

namespace Vorchestra.Domain.DataModels;

public class Tenant : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhoneNumber { get; set; }
    public string AdminEmail { get; set; }
    public string BusinessEmail { get; set; }
    public string Domain { get; set; }
    public string Status { get; set; } = TenantStatus.PENDING;
    public DateTimeOffset? SuspendedAt { get; set; } = null;
    public string SuspensionReason { get; set; } = string.Empty;
    public DateTimeOffset? TrialEndsAt { get; set; } = null;
    public Guid? PlanId { get; set; } = null;
    public DateTimeOffset? SubscriptionStartDate { get; set; } = null;
    public DateTimeOffset? SubscriptionEndDate { get; set; } = null;
    public string BillingCycle { get; set; } = Constants.BillingCycle.MONTHLY;
    public string Slug { get; set; } 
    public string Identifier { get; set; }
    public bool IsSetupComplete { get; set; } = false;
    public DateTimeOffset? OnboardedAt { get; set; } = null;
    public Guid? ServerId { get; set; } = null;
}
