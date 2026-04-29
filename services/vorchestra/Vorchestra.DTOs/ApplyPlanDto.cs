namespace Vorchestra.DTOs;

public class ApplyPlanDto
{
    public Guid TenantProjectId { get; set; }
    public Guid PlanId { get; set; }
    public string BillingCycle { get; set; }
}
