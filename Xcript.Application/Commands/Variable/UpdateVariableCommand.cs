using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Commands.Variable;

public class UpdateVariableCommand : UpdateVariableDto, IRequest<ResponseModel<Guid>>
{
}
public class UpdateVariableCommandHandler : IRequestHandler<UpdateVariableCommand, ResponseModel<Guid>>
{
    private readonly IVariableService _variableService;
    public UpdateVariableCommandHandler(IVariableService variableService)
    {
        _variableService = variableService;
    }
    public async Task<ResponseModel<Guid>> Handle(UpdateVariableCommand request, CancellationToken cancellationToken)
    {
        return await _variableService.UpdateAsync(request.Id, request, cancellationToken);
    }
}
