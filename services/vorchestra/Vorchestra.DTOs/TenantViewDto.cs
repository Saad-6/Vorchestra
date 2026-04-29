namespace Vorchestra.DTOs;

public class TenantViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = null!;
    public string AdminEmail { get; set; } = null!;
    public string BusinessEmail { get; set; } = string.Empty;
    
}

public class TenantProjectDto
{
    public Guid ProjectId { get; set; }
    public Guid ProjectName { get; set; }
    public Guid? ServerId { get; set; }
    public string? Domain { get; set; } = null;

    public TenantSubscriptionDto? Subscription { get; set; }
}

public class TenantSubscriptionDto
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = null!;
    public string BillingCycle { get; set; } = null!;
    public bool IsFreeTrial { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string Status { get; set; } = null!;
}
