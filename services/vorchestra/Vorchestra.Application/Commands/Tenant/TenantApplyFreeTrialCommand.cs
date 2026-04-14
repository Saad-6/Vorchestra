using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.Application.Commands.Tenant;

public class TenantApplyFreeTrialCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantId { get; set; }
}
public class TenantApplyFreeTrialCommandHandler : IRequestHandler<TenantApplyFreeTrialCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    public TenantApplyFreeTrialCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<string>> Handle(TenantApplyFreeTrialCommand request, CancellationToken cancellationToken)
    {
        var trialEndDate = DateTimeOffset.UtcNow.AddDays(30); // Hardcoded for now, will add it to config later
        return await _tenantService.ApplyFreeTrialAsync(request.TenantId, trialEndDate, cancellationToken);
    }
}
