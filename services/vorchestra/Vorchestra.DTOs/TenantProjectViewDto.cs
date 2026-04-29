namespace Vorchestra.DTOs;

public class TenantProjectViewDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ServerId { get; set; }
    public string? Domain { get; set; }
    public string? ConnectionString { get; set; }
    public int? AssignedPort { get; set; }
    public string Status { get; set; } = null!;
    public bool IsSetupComplete { get; set; }
    public DateTimeOffset? OnboardedAt { get; set; }
    public DateTimeOffset? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
}
