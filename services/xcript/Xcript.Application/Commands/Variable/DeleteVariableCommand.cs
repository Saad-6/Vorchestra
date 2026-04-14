using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.Variable;

public class DeleteVariableCommand : IRequest<ResponseModel<string>>
{
    public Guid Id { get; set; }
}

public class DeleteVariableCommandHandler : IRequestHandler<DeleteVariableCommand, ResponseModel<string>>
{
    private readonly IVariableService _variableService;
    public DeleteVariableCommandHandler(IVariableService variableService)
    {
        _variableService = variableService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteVariableCommand request, CancellationToken cancellationToken)
    {
        return await _variableService.DeleteAsync(request.Id, cancellationToken);
    }
}