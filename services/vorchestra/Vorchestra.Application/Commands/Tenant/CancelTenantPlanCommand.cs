using MediatR;
using Shared.Application.Models;
using Shared.Domain.Constants;
using Vorchestra.Application.Helpers;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;

namespace Vorchestra.Application.Commands.Tenant;

public class CancelTenantPlanCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantId { get; set; }
}

public class CancelTenantPlanCommandHandler : IRequestHandler<CancelTenantPlanCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    private readonly IServerService _serverService;
    private readonly ITenantEventPublisher _tenantEventPublisher;
    public CancelTenantPlanCommandHandler(ITenantService tenantService, IServerService serverService, ITenantEventPublisher tenantEventPublisher)
    {
        _tenantService = tenantService;
        _serverService = serverService;
        _tenantEventPublisher = tenantEventPublisher;
    }
    public async Task<ResponseModel<string>> Handle(CancelTenantPlanCommand request, CancellationToken cancellationToken)
    {
        var response = await _tenantService.CancelSubscriptionAsync(request.TenantId, cancellationToken);
        
        if(!response.Success)
            return response;

        var workflowsResponse = await _tenantEventPublisher.GetWorkflowsAsync(WorkflowTrigger.Project.TENANT_SUSPENDED);

        if(!workflowsResponse.Success)
            return Utility.MapResponse(workflowsResponse);

        var tenantResponse = await _tenantService.GetTenantContextByIdAsync(request.TenantId, cancellationToken);

        if(!tenantResponse.Success)
            return Utility.MapResponse(tenantResponse);

        var serverResponse = await _serverService.GetServerContextByIdAsync(request.TenantId, cancellationToken);  

        if(!serverResponse.Success)
            return Utility.MapResponse(serverResponse);

        var workflows = workflowsResponse?.Data?.Workflows;

        var groupIds = workflows?.OrderBy(w => w.Order).SelectMany(w => w.GroupIds).ToList();

        var tenantContext = tenantResponse.Data;

        var serverContext = serverResponse.Data;

        var publishResponse = await _tenantEventPublisher.PublishEventAsync(serverContext!, groupIds!, tenantContext);

        return publishResponse;
    }
}
