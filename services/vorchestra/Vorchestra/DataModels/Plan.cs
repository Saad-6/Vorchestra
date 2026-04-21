using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class Plan : BaseEntity
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public int MaxRam { get; set; }
    public int MaxStorageGb { get; set; }
    public int MaxProducts { get; set; }
    public bool IsActive { get; set; }
}