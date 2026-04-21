using Shared.Application.Models;
using Shared.Contracts.ResponseModels;
using Shared.DTO;

namespace Vorchestra.Application.Producers;

public interface ITenantEventPublisher
{
    Task<ResponseModel<WorkflowsByTriggerResponse>> GetWorkflowsAsync(string trigger);
    Task<ResponseModel<string>> PublishEventAsync(ServerContextDto server, List<Guid> groupIds, TenantContextDto? tenant = null);
}
