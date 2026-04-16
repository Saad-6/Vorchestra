namespace Shared.Contracts.ResponseModels;

public class ScriptsByIdsResponse
{
    public string GroupName { get; set; } = string.Empty;
    public List<ScriptResponse> Scripts { get; set; } = new List<ScriptResponse>();
}

public class ScriptResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<ScriptVariableInfo> Variables { get; set; } = [];
}

/// <summary>
/// Carries the variable Name (placeholder in script content) and Source (runtime resolution key)
/// so Vbaton can substitute {{name}} → resolved value without calling back to Xcript.
/// </summary>
public class ScriptVariableInfo
{
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}