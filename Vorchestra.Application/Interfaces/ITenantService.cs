using Shared.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface ITenantService
{
    Task<PaginatedResponseModel<TenantViewDto>> GetPaginatedTenantsAsync(FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateTenantAsync(CreateTenantDto tenant, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> UpdateTenantAsync(UpdateTenantDto tenant, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> ApplyFreeTrialAsync(Guid tenantId, DateTimeOffset trialEndDate, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> ApplyPlanAsync(ApplyPlanDto apply, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> CancelSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> SuspendTenantAsync(Guid tenantId, string reason, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> ReactivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
