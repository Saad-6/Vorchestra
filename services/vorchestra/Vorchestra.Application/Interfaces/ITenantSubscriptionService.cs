using Shared.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface ITenantSubscriptionService
{
    Task<ResponseModel<string>> ApplyPlanAsync(ApplyPlanDto dto, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> ApplyFreeTrialAsync(Guid tenantProjectId, DateTimeOffset trialEndDate, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> CancelSubscriptionAsync(Guid tenantProjectId, CancellationToken cancellationToken = default);
}
