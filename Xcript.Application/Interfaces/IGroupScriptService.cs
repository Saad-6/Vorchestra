using Shared.Application.Models;

namespace Xcript.Application.Interfaces;

public interface IGroupScriptService
{
    Task<ResponseModel<string>> AddScriptToGroupAsync(Guid scriptId, Guid groupId, int order);
    Task<ResponseModel<string>> RemoveScriptFromGroupAsync(Guid scriptId, Guid groupId);
}
