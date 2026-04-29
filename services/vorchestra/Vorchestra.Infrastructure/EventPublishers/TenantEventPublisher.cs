using MassTransit;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Shared.DTO;
using Vorchestra.Application.Producers;

namespace Vorchestra.Infrastructure.EventPublishers;

public class TenantEventPublisher : ITenantEventPublisher
{
    private readonly IRequestClient<ProjectWorkflowsByTriggerRequest> _requestClient;
    private readonly IRequestClient<ExecutionRequestDto> _executionClient;

    public TenantEventPublisher(IRequestClient<ProjectWorkflowsByTriggerRequest> requestClient, IRequestClient<ExecutionRequestDto> executionClient)
    {
        _requestClient = requestClient;
        _executionClient = executionClient;
    }
    public async Task<ResponseModel<WorkflowsByTriggerResponse>> GetProjectWorkflowsAsync(string trigger, Guid projectId)
    {
        var response = await _requestClient.GetResponse<ResponseModel<WorkflowsByTriggerResponse>>(new ProjectWorkflowsByTriggerRequest { Trigger = trigger, ProjectId = projectId });

        return response.Message;
    }

    public async Task<ResponseModel<string>> PublishEventAsync(ServerContextDto server, List<Guid> groupIds, TenantContextDto tenant)
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
