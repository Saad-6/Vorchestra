using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.Application.Vaidators;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Tenant;

public class UpdateTenantCommand : UpdateTenantDto, IRequest<ResponseModel<string>>
{
}
public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, ResponseModel<string>>
{
    private readonly ITenantService _tenantService;
    public UpdateTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<string>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<TenantStatus>(request.Status);

        return await _tenantService.UpdateTenantAsync(request, cancellationToken);
    }
}
