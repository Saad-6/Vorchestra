using Shared.Application.Models;
using Shared.DTO;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface ITenantProjectService
{
    Task<ResponseModel<Guid>> CreateTenantProjectAsync(CreateTenantProjectDto dto, CancellationToken cancellationToken = default);
    Task<PaginatedResponseModel<TenantProjectViewDto>> GetTenantProjectsAsync(Guid tenantId, FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<TenantProjectViewDto>> GetTenantProjectByIdAsync(Guid tenantProjectId, CancellationToken cancellationToken = default);
    Task<ResponseModel<TenantContextDto>> GetTenantContextAsync(Guid tenantProjectId, CancellationToken cancellationToken = default);
    Task<ResponseModel<List<TenantProjectViewDto>>> GetRunningTenantProjectsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> SuspendTenantProjectAsync(Guid tenantProjectId, string reason, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> ReactivateTenantProjectAsync(Guid tenantProjectId, CancellationToken cancellationToken = default);
}
