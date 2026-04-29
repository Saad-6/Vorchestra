using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Tenant;

public class TenantsPaginatedQuery : IRequest<ResponseModel<List<TenantViewDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; }
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
        var filterModel = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>()
            {
                {nameof(Domain.DataModels.Tenant.Name), request.SearchTerm ?? string.Empty}
            }
        };

        return await _tenantService.GetPaginatedTenantsAsync(filterModel, cancellationToken);
    }
}