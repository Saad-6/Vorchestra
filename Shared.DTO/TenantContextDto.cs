namespace Shared.DTO;

public class TenantContextDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string Domain { get; set; }
    public string ConnectionString { get; set; }
    public int Port { get; set; }
}
