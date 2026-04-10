using Shared.Application.Models;
using Xcript.DTOs;

namespace Xcript.Application.Interfaces;

public interface IGroupService
{
    Task<ResponseModel<string>> CreateGroupAsync(CreateGroupDto group);
    Task<ResponseModel<string>> UpdateGroupAsync(UpdateGroupDto group);
    Task<ResponseModel<string>> DeleteGroupAsync(int groupId);
    Task<ResponseModel<GroupViewDto>> GetGroupByIdAsync(string groupId);
    Task<PaginatedResponseModel<GroupViewDto>> GetPaginatedGroupsAsync(string? name = null);
}
