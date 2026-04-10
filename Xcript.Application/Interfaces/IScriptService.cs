using Shared.Application.Models;
using Xcript.DTOs;

namespace Xcript.Application.Interfaces;

public interface IScriptService
{
    Task<ResponseModel<string>> CreateScriptAsync(CreateScriptDto script);
    Task<ResponseModel<string>> UpdateScriptAsync(UpdateScriptDto script);
    Task<ResponseModel<string>> DeleteScriptAsync(string scriptId);
    Task<ResponseModel<ScriptViewDto>> GetScriptByIdAsync(string scriptId);
    Task<PaginatedResponseModel<ScriptViewDto>> GetPaginatedScriptsAsync(string? name = null, string? content = null);
    Task<ResponseModel<string>> AddScriptToGroupAsync(string scriptId, int groupId, int order);
    Task<ResponseModel<string>> RemoveScriptFromGroupAsync(string scriptId, int groupId);
}
