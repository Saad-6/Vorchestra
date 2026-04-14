using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Queries.Group;

public class GroupByIdQuery : IRequest<ResponseModel<GroupViewDto>>
{
    public Guid Id { get; set; }
}
public class GroupByIdQueryHandler : IRequestHandler<GroupByIdQuery, ResponseModel<GroupViewDto>>
{
    private readonly IGroupService _groupService;
    public GroupByIdQueryHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    public async Task<ResponseModel<GroupViewDto>> Handle(GroupByIdQuery request, CancellationToken cancellationToken)
    {
        return await _groupService.GetGroupByIdAsync(request.Id, cancellationToken);
    }
}