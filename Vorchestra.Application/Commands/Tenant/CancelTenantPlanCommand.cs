using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;

namespace Vorchestra.Application.Commands.Tenant;

public class CancelTenantPlanCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantId { get; set; }
}

public class CancelTenantPlanCommandHandler : IRequestHandler<CancelTenantPlanCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    public CancelTenantPlanCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<string>> Handle(CancelTenantPlanCommand request, CancellationToken cancellationToken)
    {
        return await _tenantService.CancelSubscriptionAsync(request.TenantId, cancellationToken);
    }
}
