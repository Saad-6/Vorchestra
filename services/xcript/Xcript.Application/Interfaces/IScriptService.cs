using Shared.Application.Models;
using Xcript.DTOs;

namespace Xcript.Application.Interfaces;

public interface IScriptService
{
    Task<ResponseModel<Guid>> CreateScriptAsync(CreateScriptDto script, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> UpdateScriptAsync(UpdateScriptDto script, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteScriptAsync(Guid scriptId, CancellationToken cancellationToken = default);
    Task<ResponseModel<ScriptViewDto>> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken = default);
    Task<ResponseModel<List<ScriptViewDto>>> GetScriptsByIdsAsync(List<Guid> scriptIds, CancellationToken cancellationToken = default);
    Task<PaginatedResponseModel<ScriptViewDto>> GetPaginatedScriptsAsync(FilterModel filter, CancellationToken cancellationToken = default);
}
