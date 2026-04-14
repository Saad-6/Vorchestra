using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Queries.Script;

public class PaginatedScriptQuery : IRequest<PaginatedResponseModel<ScriptViewDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SearchTerm { get; set; }
}
public class PaginatedScriptQueryHandler : IRequestHandler<PaginatedScriptQuery, PaginatedResponseModel<ScriptViewDto>>
{
    private readonly IScriptService _scriptService;
    public PaginatedScriptQueryHandler(IScriptService scriptService)
    {
        _scriptService = scriptService;
    }
    public async Task<PaginatedResponseModel<ScriptViewDto>> Handle(PaginatedScriptQuery request, CancellationToken cancellationToken)
    {
        var filter = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>
            {
                { nameof(Domain.DataModels.Script.Name), request.SearchTerm ?? string.Empty }
            }
        };
        return await _scriptService.GetPaginatedScriptsAsync(filter, cancellationToken);
    }
}