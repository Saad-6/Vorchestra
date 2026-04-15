namespace Shared.Contracts.RequestModels;

public class ScriptsByIdsRequest
{
    public List<Guid> ScriptIds { get; set; } = new List<Guid>();
}
