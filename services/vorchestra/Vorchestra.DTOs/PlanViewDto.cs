namespace Vorchestra.DTOs;

public class PlanViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public int MaxRam { get; set; }
    public int MaxStorageGb { get; set; }
    public int MaxProducts { get; set; }
    public bool IsActive { get; set; }
}
