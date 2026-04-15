using Shared.Contracts.ResponseModels;
using Shared.DTO;

namespace Vbaton.Application.Models;

public class NormalizedExecutionRequest
{
    public ServerContextDto Server { get; set; } = new();
    public List<ScriptResponse>?Scripts { get; set; } = new();
}
