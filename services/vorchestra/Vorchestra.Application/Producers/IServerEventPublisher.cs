using Shared.Application.Models;
using Shared.Contracts.ResponseModels;
using Shared.DTO;

namespace Vorchestra.Application.Producers;

public interface IServerEventPublisher
{
    Task<ResponseModel<WorkflowsByTriggerResponse>> GetServerWorkflowsAsync(string trigger, Guid serverId);

    Task<ResponseModel<WorkflowsByTriggerResponse>> GetServerWorkflowByIdAsync(Guid workflowId);

    Task<ResponseModel<string>> PublishEventAsync(ServerContextDto server, List<Guid> groupIds);
}
