using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.Application.Commands.Tenant;

public class ReActivateTenantCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantId { get; set; }
}
public class ReActivateTenantCommandHandler : IRequestHandler<ReActivateTenantCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    public ReActivateTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<string>> Handle(ReActivateTenantCommand request, CancellationToken cancellationToken)
    {
        return await _tenantService.ReactivateTenantAsync(request.TenantId, cancellationToken);
    }
}
