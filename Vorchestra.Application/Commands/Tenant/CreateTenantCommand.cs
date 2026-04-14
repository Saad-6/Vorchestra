using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Tenant;

public class CreateTenantCommand : CreateTenantDto, IRequest<ResponseModel<Guid>>
{
}
public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ResponseModel<Guid>>
{
    private readonly ITenantService _tenantService;
    public CreateTenantCommandHandler(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        return await _tenantService.CreateTenantAsync(request, cancellationToken);
    }
}