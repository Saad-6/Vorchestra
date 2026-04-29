using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.Application.Commands.Tenant;

public class ReActivateTenantCommand : IRequest<ResponseModel<string>>
{
    public Guid TenantProjectId { get; set; }
}

public class ReActivateTenantCommandHandler : IRequestHandler<ReActivateTenantCommand, ResponseModel<string>>
{
    private readonly ITenantProjectService _tenantProjectService;

    public ReActivateTenantCommandHandler(ITenantProjectService tenantProjectService)
    {
        _tenantProjectService = tenantProjectService;
    }

    public async Task<ResponseModel<string>> Handle(ReActivateTenantCommand request, CancellationToken cancellationToken)
    {
        return await _tenantProjectService.ReactivateTenantProjectAsync(request.TenantProjectId, cancellationToken);
    }
}
