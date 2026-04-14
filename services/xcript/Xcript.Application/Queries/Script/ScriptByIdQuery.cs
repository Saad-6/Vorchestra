using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Queries.Script;

public class ScriptByIdQuery : IRequest<ResponseModel<ScriptViewDto>>
{
    public Guid Id { get; set; }
}
public class ScriptByIdQueryHandler : IRequestHandler<ScriptByIdQuery, ResponseModel<ScriptViewDto>>
{
    private readonly IScriptService _scriptService;
    public ScriptByIdQueryHandler(IScriptService scriptService)
    {
        _scriptService = scriptService;
    }
    public async Task<ResponseModel<ScriptViewDto>> Handle(ScriptByIdQuery request, CancellationToken cancellationToken)
    {
        return await _scriptService.GetScriptByIdAsync(request.Id, cancellationToken);
    }
}