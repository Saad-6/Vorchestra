using MediatR;
using Shared.Application.Models;
using Shared.Application.Vaidators;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Tenant;

public class TenantsPaginatedQuery : IRequest<ResponseModel<List<TenantViewDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Status { get; set; }
    public string? Name { get; set; }
}
public class TenantsPaginatedQueryHandler : IRequestHandler<TenantsPaginatedQuery, ResponseModel<List<TenantViewDto>>>
{
    private readonly ITenantService _tenantService;
    public TenantsPaginatedQueryHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<List<TenantViewDto>>> Handle(TenantsPaginatedQuery request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<TenantStatus>(request.Status);

        return await _tenantService.GetPaginatedTenantsAsync(request.PageNumber, request.PageSize, request.Status, request.Name, cancellationToken);
    }
}