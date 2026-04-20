namespace Shared.DTO;

public class BaseWorkflowDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
    public string Trigger { get; set; } = null!;
    public int Order { get; set; }
}
