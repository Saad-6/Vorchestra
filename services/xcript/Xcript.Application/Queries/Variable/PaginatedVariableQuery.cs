using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Queries.Variable;

public class PaginatedVariableQuery : IRequest<PaginatedResponseModel<VariableViewDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}
public class PaginatedVariableQueryHandler : IRequestHandler<PaginatedVariableQuery, PaginatedResponseModel<VariableViewDto>>
{
    private readonly IVariableService _variableService;
    public PaginatedVariableQueryHandler(IVariableService variableService)
    {
        _variableService = variableService;
    }
    public async Task<PaginatedResponseModel<VariableViewDto>> Handle(PaginatedVariableQuery request, CancellationToken cancellationToken)
    {
        var filter = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>()
            {
                { nameof(Domain.DataModels.Variable.Name), request.SearchTerm ?? string.Empty }
            }
        };
        return await _variableService.GetPaginatedVariablesAsync(filter, cancellationToken);
    }
}
