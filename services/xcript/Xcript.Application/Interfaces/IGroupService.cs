using Shared.Application.Models;
using Xcript.DTOs;

namespace Xcript.Application.Interfaces;

public interface IGroupService
{
    Task<ResponseModel<Guid>> CreateGroupAsync(CreateGroupDto group, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> UpdateGroupAsync(UpdateGroupDto group, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteGroupAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<ResponseModel<GroupViewDto>> GetGroupByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<ResponseModel<List<GroupViewDto>>> GetGroupsByIdsAsync(List<Guid> groupIds,CancellationToken cancellationToken = default);
    Task<PaginatedResponseModel<GroupViewDto>> GetPaginatedGroupsAsync(FilterModel filter, CancellationToken cancellationToken = default);
}
