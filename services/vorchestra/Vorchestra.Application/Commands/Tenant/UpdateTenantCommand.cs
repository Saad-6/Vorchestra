using MediatR;
using Shared.Application.Models;
using Shared.Application.Vaidators;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Tenant;

public class UpdateTenantCommand : UpdateTenantDto, IRequest<ResponseModel<Guid>>
{
}
public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, ResponseModel<Guid>>
{
    private readonly ITenantService _tenantService;
    public UpdateTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<Guid>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<TenantStatus>(request.Status);

        return await _tenantService.UpdateTenantAsync(request, cancellationToken);
    }
}
