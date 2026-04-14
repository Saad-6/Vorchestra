using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Queries.VariableSource;
public class AllVariableSourcesQuery : IRequest<ResponseModel<IEnumerable<string>>>
{
}
public class AllVariableSourcesQueryHandler : IRequestHandler<AllVariableSourcesQuery, ResponseModel<IEnumerable<string>>>
{
    private readonly IVariableSourceService _variableSourceService;
    public AllVariableSourcesQueryHandler(IVariableSourceService variableSourceService)
    {
        _variableSourceService = variableSourceService;
    }
    public async Task<ResponseModel<IEnumerable<string>>> Handle(AllVariableSourcesQuery request, CancellationToken cancellationToken)
    {
        return await _variableSourceService.GetAvailableSourcesAsync();
    }
}