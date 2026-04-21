using MediatR;
using Shared.Application.Models;
using Shared.Application.Vaidators;
using Shared.Domain.Constants;
using Vorchestra.Application.Helpers;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Tenant;

public class TenantSubscribePlanCommand : ApplyPlanDto, IRequest<ResponseModel<string>>
{
}

public class TenantSubscribePlanCommandHandler : IRequestHandler<TenantSubscribePlanCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    private readonly IServerService _serverService;
    private readonly ITenantEventPublisher _tenantEventPublisher;
    public TenantSubscribePlanCommandHandler(ITenantService tenantService, IServerService serverService, ITenantEventPublisher tenantEventPublisher)
    {
        _tenantService = tenantService;
        _serverService = serverService;
        _tenantEventPublisher = tenantEventPublisher;
    }
    public async Task<ResponseModel<string>> Handle(TenantSubscribePlanCommand request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<BillingCycle>(request.BillingCycle);

        var response = await _tenantService.ApplyPlanAsync(request, cancellationToken);

        if(!response.Success)
            return response;

        var workflowsResponse = await _tenantEventPublisher.GetWorkflowsAsync(WorkflowTrigger.Project.TENANT_SUBSCRIBED);

        if (!workflowsResponse.Success)
            return Utility.MapResponse(workflowsResponse);

        var tenantResponse = await _tenantService.GetTenantContextByIdAsync(request.TenantId, cancellationToken);

        if (!tenantResponse.Success)
            return Utility.MapResponse(tenantResponse);

        var serverResponse = await _serverService.GetServerContextByIdAsync(request.ServerId!.Value, cancellationToken);

        if (!serverResponse.Success)
            return Utility.MapResponse(serverResponse);

        var workflows = workflowsResponse?.Data?.Workflows;

        var groupIds = workflows?.OrderBy(w => w.Order).SelectMany(w => w.GroupIds).ToList();

        var tenantContext = tenantResponse.Data;

        var serverContext = serverResponse.Data;

        var publishResponse = await _tenantEventPublisher.PublishEventAsync(serverContext!, groupIds!, tenantContext);

        return publishResponse;
    }
}
