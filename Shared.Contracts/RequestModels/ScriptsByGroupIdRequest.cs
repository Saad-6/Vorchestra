namespace Shared.Contracts.RequestModels;

public class ScriptsByGroupIdRequest
{
    public List<Guid>? GroupIds { get; set; }
}
