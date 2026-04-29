namespace Vorchestra.DTOs;

public class CreateTenantProjectDto
{
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ServerId { get; set; }
    public string? Domain { get; set; }
    public string? ConnectionString { get; set; }
    public int? AssignedPort { get; set; }
}
