namespace Shared.DTO;

public class ExecutionRequestDto
{
    public ServerContextDto Server { get; set; } = new();

    public TenantContextDto Tenant { get; set; } = new();

    public Guid? GroupId { get; set; } 

    public List<Guid>? ScriptIds { get; set; }    

}
