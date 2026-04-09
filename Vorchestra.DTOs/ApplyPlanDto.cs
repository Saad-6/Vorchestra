namespace Vorchestra.DTOs;

public class ApplyPlanDto
{
    public Guid PlanId { get; set; }
    public Guid TenantId { get; set; }
    public Guid? ServerId { get; set; }
    public string BillngCycle { get; set; }

}
