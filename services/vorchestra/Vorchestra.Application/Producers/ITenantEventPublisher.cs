using Shared.Application.Models;
using Shared.Contracts.ResponseModels;
using Shared.DTO;

namespace Vorchestra.Application.Producers;

public interface ITenantEventPublisher
{
    Task<ResponseModel<WorkflowsByTriggerResponse>> GetProjectWorkflowsAsync(string trigger, Guid projectId);
    Task<ResponseModel<string>> PublishEventAsync(ServerContextDto server, List<Guid> groupIds, TenantContextDto tenant);
}
