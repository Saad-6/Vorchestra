using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.ScriptVariable;

public class AssignVariableToScriptCommand : IRequest<ResponseModel<string>>
{
    public Guid ScriptId { get; set; }
    public Guid VariableId { get; set; }
}
public class AssignVariableToScriptCommandHandler : IRequestHandler<AssignVariableToScriptCommand, ResponseModel<string>>
{
    private readonly IScriptVariableService _scriptVariableService;
    public AssignVariableToScriptCommandHandler(IScriptVariableService scriptVariableService)
    {
        _scriptVariableService = scriptVariableService;
    }
    public async Task<ResponseModel<string>> Handle(AssignVariableToScriptCommand request, CancellationToken cancellationToken)
    {
        return await _scriptVariableService.AssignVariableToScriptAsync(request.ScriptId, request.VariableId, cancellationToken);
    }
}