using Shared.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface ITenantService
{
    Task<PaginatedResponseModel<TenantViewDto>> GetPaginatedTenantsAsync(FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateTenantAsync(CreateTenantDto tenant, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> UpdateTenantAsync(UpdateTenantDto tenant, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
