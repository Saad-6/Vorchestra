using Shared.Application.Models;
using Vbaton.Application.Models;

namespace Vbaton.Application.Interfaces;

public interface ISshService
{
    Task<ResponseModel<List<ScriptOutputModel>>> ExecuteCommandsAsync(NormalizedExecutionRequest request);
}
