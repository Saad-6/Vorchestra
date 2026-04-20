using Shared.DTO;
using Vbaton.Application.Models;

namespace Vbaton.Application.Interfaces;

public interface ILogService
{
    Task LogAsync(ExecutionRequestDto request, Guid? groupId, string output, bool succeeded, List<ScriptOutputModel> scriptOutputs);
}
