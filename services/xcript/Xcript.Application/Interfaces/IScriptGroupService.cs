using Shared.Application.Models;

namespace Xcript.Application.Interfaces;

public interface IScriptGroupService
{
    Task<ResponseModel<string>> AddScriptToGroupAsync(Guid scriptId, Guid groupId, int order, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> RemoveScriptFromGroupAsync(Guid scriptId, Guid groupId, CancellationToken cancellationToken = default);
}
