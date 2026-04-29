using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.Application.Commands.Tenant;

public class TenantApplyFreeTrialCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantProjectId { get; set; }
}

public class TenantApplyFreeTrialCommandHandler : IRequestHandler<TenantApplyFreeTrialCommand, ResponseModel<string>>
{
    private readonly ITenantSubscriptionService _subscriptionService;

    public TenantApplyFreeTrialCommandHandler(ITenantSubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public async Task<ResponseModel<string>> Handle(TenantApplyFreeTrialCommand request, CancellationToken cancellationToken)
    {
        var trialEndDate = DateTimeOffset.UtcNow.AddDays(30); // TODO: move to config
        return await _subscriptionService.ApplyFreeTrialAsync(request.TenantProjectId, trialEndDate, cancellationToken);
    }
}
