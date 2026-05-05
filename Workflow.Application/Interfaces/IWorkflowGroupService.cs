using Shared.Application.Models;

namespace Workflow.Application.Interfaces;

public interface IWorkflowGroupService
{
    Task<ResponseModel<string>> RemoveGroupFromWorkflowAsync(Guid workflowId, Guid groupId, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> AddGroupToWorkflowAsync(Guid workflowId, Guid groupId, string groupName, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> ChangeGroupOrderInWorkflowAsync(Guid workflowId, Guid groupId, int newOrder, CancellationToken cancellationToken = default);
}
