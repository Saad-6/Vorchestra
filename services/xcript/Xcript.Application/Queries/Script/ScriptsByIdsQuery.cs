using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Queries.Script;

public class ScriptsByIdsQuery : IRequest<ResponseModel<List<ScriptViewDto>>>
{
    public List<Guid> ScriptIds { get; set; }
}
public class ScriptsByIdsQueryHandler : IRequestHandler<ScriptsByIdsQuery, ResponseModel<List<ScriptViewDto>>>
{
    private readonly IScriptService _scriptService;
    public ScriptsByIdsQueryHandler(IScriptService scriptService)
    {
        _scriptService = scriptService;
    }
    public async Task<ResponseModel<List<ScriptViewDto>>> Handle(ScriptsByIdsQuery request, CancellationToken cancellationToken)
    {
        return await _scriptService.GetScriptsByIdsAsync(request.ScriptIds, cancellationToken);
    }
}
