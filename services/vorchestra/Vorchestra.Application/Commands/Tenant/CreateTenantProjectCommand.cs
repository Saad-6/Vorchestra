using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Tenant;

public class CreateTenantProjectCommand : CreateTenantProjectDto, IRequest<ResponseModel<Guid>>
{
}

public class CreateTenantProjectCommandHandler : IRequestHandler<CreateTenantProjectCommand, ResponseModel<Guid>>
{
    private readonly ITenantProjectService _tenantProjectService;

    public CreateTenantProjectCommandHandler(ITenantProjectService tenantProjectService)
    {
        _tenantProjectService = tenantProjectService;
    }

    public async Task<ResponseModel<Guid>> Handle(CreateTenantProjectCommand request, CancellationToken cancellationToken)
    {
        return await _tenantProjectService.CreateTenantProjectAsync(request, cancellationToken);
    }
}
