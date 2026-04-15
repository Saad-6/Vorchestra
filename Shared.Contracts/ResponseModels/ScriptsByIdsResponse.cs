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
}