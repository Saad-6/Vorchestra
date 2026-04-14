using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.ScriptVariable;

public class RemoveVariableFromScriptCommand : IRequest<ResponseModel<string>>
{
    public Guid ScriptId { get; set; }
    public Guid VariableId { get; set; }
}

public class RemoveVariableFromScriptCommandHandler : IRequestHandler<RemoveVariableFromScriptCommand, ResponseModel<string>>
{
    private readonly IScriptVariableService _scriptVariableService;
    public RemoveVariableFromScriptCommandHandler(IScriptVariableService scriptVariableService)
    {
        _scriptVariableService = scriptVariableService;
    }
    public async Task<ResponseModel<string>> Handle(RemoveVariableFromScriptCommand request, CancellationToken cancellationToken)
    {
        return await _scriptVariableService.RemoveVariableFromScriptAsync(request.ScriptId, request.VariableId, cancellationToken);
    }
}