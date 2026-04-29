using MediatR;
using Shared.Application.Models;
using Shared.Domain.Constants;
using Vorchestra.Application.Helpers;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;

namespace Vorchestra.Application.Commands.Tenant;

public class CancelTenantPlanCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantProjectId { get; set; }
}

public class CancelTenantPlanCommandHandler : IRequestHandler<CancelTenantPlanCommand, ResponseModel<string>>
{
    private readonly ITenantSubscriptionService _subscriptionService;
    private readonly ITenantProjectService _tenantProjectService;
    private readonly IServerService _serverService;
    private readonly ITenantEventPublisher _tenantEventPublisher;

    public CancelTenantPlanCommandHandler(
        ITenantSubscriptionService subscriptionService,
        ITenantProjectService tenantProjectService,
        IServerService serverService,
        ITenantEventPublisher tenantEventPublisher)
    {
        _subscriptionService = subscriptionService;
        _tenantProjectService = tenantProjectService;
        _serverService = serverService;
        _tenantEventPublisher = tenantEventPublisher;
    }

    public async Task<ResponseModel<string>> Handle(CancelTenantPlanCommand request, CancellationToken cancellationToken)
    {
        var tenantProjectResponse = await _tenantProjectService.GetTenantProjectByIdAsync(request.TenantProjectId, cancellationToken);

        if (!tenantProjectResponse.Success)
            return Utility.MapResponse(tenantProjectResponse);

        var response = await _subscriptionService.CancelSubscriptionAsync(request.TenantProjectId, cancellationToken);

        if (!response.Success)
            return response;

        var workflowsResponse = await _tenantEventPublisher.GetProjectWorkflowsAsync(WorkflowTrigger.Project.TENANT_SUSPENDED, tenantProjectResponse.Data!.ProjectId);

        if (!workflowsResponse.Success)
            return Utility.MapResponse(workflowsResponse);

        var tenantContextResponse = await _tenantProjectService.GetTenantContextAsync(request.TenantProjectId, cancellationToken);

        if (!tenantContextResponse.Success)
            return Utility.MapResponse(tenantContextResponse);

        var serverId = tenantProjectResponse.Data!.ServerId;

        if (!serverId.HasValue)
            return new ResponseModel<string> { Success = false, Message = "No server assigned to this tenant project." };

        var serverResponse = await _serverService.GetServerContextByIdAsync(serverId.Value, cancellationToken);

        if (!serverResponse.Success)
            return Utility.MapResponse(serverResponse);

        var groupIds = workflowsResponse.Data?.Workflows?.OrderBy(w => w.Order).SelectMany(w => w.GroupIds).ToList();

        var publishResponse = await _tenantEventPublisher.PublishEventAsync(serverResponse.Data!, groupIds!, tenantContextResponse.Data!);

        return publishResponse;
    }
}
