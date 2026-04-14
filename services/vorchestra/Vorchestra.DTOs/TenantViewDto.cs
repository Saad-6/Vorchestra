namespace Vorchestra.DTOs;

public class TenantViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhoneNumber { get; set; }
    public string AdminEmail { get; set; }
    public string BusinessEmail { get; set; }
    public string Domain { get; set; }
    public string Status { get; set; } 
    public DateTimeOffset? SuspendedAt { get; set; } 
    public string SuspensionReason { get; set; } 
    public DateTimeOffset? TrialEndsAt { get; set; }
    public DateTimeOffset? SubscriptionStartDate { get; set; } 
    public DateTimeOffset? SubscriptionEndDate { get; set; } 
    public string BillingCycle { get; set; }
    public string Slug { get; set; }
    public string Identifier { get; set; }
    public bool IsSetupComplete { get; set; }
    public DateTimeOffset? OnboardedAt { get; set; } 
    public Guid? PlanId { get; set; }
    public Guid? ServerId { get; set; }
}
