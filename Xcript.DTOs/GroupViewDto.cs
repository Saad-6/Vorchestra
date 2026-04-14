namespace Xcript.DTOs;

public class GroupViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<ScriptViewDto> Scripts { get; set; }
}
