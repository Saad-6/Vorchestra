using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Queries.Group;

public class GroupsByIdsQuery : IRequest<ResponseModel<List<GroupViewDto>>>
{
    public List<Guid> GroupIds { get; set; } = new List<Guid>();
}
public class GroupsByIdsQueryHandler : IRequestHandler<GroupsByIdsQuery, ResponseModel<List<GroupViewDto>>>
{
    private readonly IGroupService _groupService;
    public GroupsByIdsQueryHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    
    public async Task<ResponseModel<List<GroupViewDto>>> Handle(GroupsByIdsQuery request, CancellationToken cancellationToken)
    {
        return await _groupService.GetGroupsByIdsAsync(request.GroupIds, cancellationToken);
    }
}
