namespace Vorchestra.DTOs;

public class UpdateTenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhoneNumber { get; set; }
    public string AdminEmail { get; set; }
    public string BusinessEmail { get; set; }
    public string Domain { get; set; }
    public string Status { get; set; } 
    public DateTime? SuspendedAt { get; set; } = null;
    public string? SuspensionReason { get; set; } = null;
    public string? Slug { get; set; } = null;
    public string? Identifier { get; set; } = null;
}
