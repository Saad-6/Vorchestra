namespace Xcript.DTOs;

public class ScriptViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public int? Order { get; set; } = null;
    public List<ScriptVariableViewDto> Variables { get; set; } = [];
}

public class ScriptVariableViewDto
{
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}
