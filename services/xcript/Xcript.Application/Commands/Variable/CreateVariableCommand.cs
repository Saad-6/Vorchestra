using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Commands.Variable;

public class CreateVariableCommand : CreateVariableDto, IRequest<ResponseModel<Guid>>
{
}
public class CreateVariableCommandHandler : IRequestHandler<CreateVariableCommand, ResponseModel<Guid>>
{
    private readonly IVariableService _variableService;
    public CreateVariableCommandHandler(IVariableService variableService)
    {
        _variableService = variableService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateVariableCommand request, CancellationToken cancellationToken)
    {
        return await _variableService.CreateAsync(request, cancellationToken);
    }
}
