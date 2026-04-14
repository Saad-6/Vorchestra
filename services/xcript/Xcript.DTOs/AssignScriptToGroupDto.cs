namespace Xcript.DTOs;

public class AssignScriptToGroupDto
{
    public Guid ScriptId { get; set; }
    public int Order { get; set; }
    public Guid GroupId { get; set; }
}
