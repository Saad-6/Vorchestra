using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Queries.Variable;

public class VariableByIdQuery : IRequest<ResponseModel<VariableViewDto>>
{
    public Guid Id { get; set; }
}
public class VariableByIdQueryHandler : IRequestHandler<VariableByIdQuery, ResponseModel<VariableViewDto>>
{
    private readonly IVariableService _variableService;
    public VariableByIdQueryHandler(IVariableService variableService)
    {
        _variableService = variableService;
    }
    public async Task<ResponseModel<VariableViewDto>> Handle(VariableByIdQuery request, CancellationToken cancellationToken)
    {
        return await _variableService.GetByIdAsync(request.Id, cancellationToken);
    }
}
