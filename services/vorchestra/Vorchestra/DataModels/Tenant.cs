using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = null!;
    public int Identifier { get; set; } = 0;
    public string PhoneNumber { get; set; } = null!;
    public string AdminEmail { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
    public string BusinessEmail { get; set; } = null!;
}
