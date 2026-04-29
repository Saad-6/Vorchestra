using MassTransit;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Shared.DTO;
using Vorchestra.Application.Producers;

namespace Vorchestra.Infrastructure.EventPublishers;

public class ServerEventPublisher : IServerEventPublisher
{
    private readonly IRequestClient<ServerWorkflowsByTriggerRequest> _triggerClient;
    private readonly IRequestClient<ServerWorkflowByIdRequest> _byIdClient;
    private readonly IRequestClient<ExecutionRequestDto> _executionClient;

    public ServerEventPublisher(
        IRequestClient<ServerWorkflowsByTriggerRequest> triggerClient,
        IRequestClient<ServerWorkflowByIdRequest> byIdClient,
        IRequestClient<ExecutionRequestDto> executionClient)
    {
        _triggerClient = triggerClient;
        _byIdClient = byIdClient;
        _executionClient = executionClient;
    }

    public async Task<ResponseModel<WorkflowsByTriggerResponse>> GetServerWorkflowsAsync(string trigger, Guid serverId)
    {
        var response = await _triggerClient.GetResponse<ResponseModel<WorkflowsByTriggerResponse>>(
            new ServerWorkflowsByTriggerRequest { Trigger = trigger, ServerId = serverId });

        return response.Message;
    }

    public async Task<ResponseModel<WorkflowsByTriggerResponse>> GetServerWorkflowByIdAsync(Guid workflowId)
    {
        var response = await _byIdClient.GetResponse<ResponseModel<WorkflowsByTriggerResponse>>(
            new ServerWorkflowByIdRequest { WorkflowId = workflowId });

        return response.Message;
    }

    public async Task<ResponseModel<string>> PublishEventAsync(ServerContextDto server, List<Guid> groupIds)
    {
        var request = new ExecutionRequestDto
        {
            Server = server,
            GroupIds = groupIds
        };

        var response = await _executionClient.GetResponse<ResponseModel<string>>(request);

        return response.Message;
    }
}
