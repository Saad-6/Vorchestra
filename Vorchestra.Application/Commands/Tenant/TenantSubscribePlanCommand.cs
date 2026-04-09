using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.Application.Vaidators;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Tenant;

public class TenantSubscribePlanCommand : ApplyPlanDto, IRequest<ResponseModel<string>>
{
}

public class TenantSubscribePlanCommandHandler : IRequestHandler<TenantSubscribePlanCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    public TenantSubscribePlanCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<string>> Handle(TenantSubscribePlanCommand request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<BillingCycle>(request.BillngCycle);

        return await _tenantService.ApplyPlanAsync(request, cancellationToken);
    }
}
