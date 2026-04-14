using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Queries.Group;

public class PaginatedGroupQuery : IRequest<PaginatedResponseModel<GroupViewDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string? SearchTerm { get; set; }
}
public class PaginatedGroupQueryHandler : IRequestHandler<PaginatedGroupQuery, PaginatedResponseModel<GroupViewDto>>
{
    private readonly IGroupService _groupService;
    public PaginatedGroupQueryHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    public async Task<PaginatedResponseModel<GroupViewDto>> Handle(PaginatedGroupQuery request, CancellationToken cancellationToken)
    {
        var filter = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>()
            {
                { nameof(Domain.DataModels.Group.Name), request.SearchTerm ?? string.Empty }
            }
        };
        return await _groupService.GetPaginatedGroupsAsync(filter, cancellationToken);
    }
}