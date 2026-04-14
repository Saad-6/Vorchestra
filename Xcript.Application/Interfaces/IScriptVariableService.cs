using Shared.Application.Models;

namespace Xcript.Application.Interfaces;

public interface IScriptVariableService
{
    Task<ResponseModel<string>> AssignVariableToScriptAsync(Guid scriptId, Guid variableId, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> RemoveVariableFromScriptAsync(Guid scriptId, Guid variableId, CancellationToken cancellationToken = default);
}
