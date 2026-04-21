using MassTransit;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Shared.DTO;
using Vorchestra.Application.Producers;

namespace Vorchestra.Infrastructure.EventPublishers;

public class TenantEventPublisher : ITenantEventPublisher
{
    private readonly IRequestClient<WorkflowsByTriggerRequest> _requestClient;
    private readonly IRequestClient<ExecutionRequestDto> _executionClient;

    public TenantEventPublisher(IRequestClient<WorkflowsByTriggerRequest> requestClient, IRequestClient<ExecutionRequestDto> executionClient)
    {
        _requestClient = requestClient;
        _executionClient = executionClient;
    }
    public async Task<ResponseModel<WorkflowsByTriggerResponse>> GetWorkflowsAsync(string trigger)
    {
        var response = await _requestClient.GetResponse<ResponseModel<WorkflowsByTriggerResponse>>(new WorkflowsByTriggerRequest { Trigger = trigger });

        return response.Message;
    }

    public async Task<ResponseModel<string>> PublishEventAsync(ServerContextDto server, List<Guid> groupIds, TenantContextDto? tenant = null)
    {
        var request = new ExecutionRequestDto
        {
            Tenant = tenant,
            Server = server,
            GroupIds = groupIds
        };

        var response = await _executionClient.GetResponse<ResponseModel<string>>(request);

        return response.Message;
    }
}
