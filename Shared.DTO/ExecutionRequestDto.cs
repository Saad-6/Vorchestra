namespace Shared.DTO;

public class ExecutionRequestDto
{
    public ServerContextDto Server { get; set; } = new();
    public TenantContextDto? Tenant { get; set; } = new();
    public ProjectContextDto? Project { get; set; } = new();
    public List<Guid> GroupIds { get; set; } = new();
    public List<Guid>? ScriptIds { get; set; }    

}
