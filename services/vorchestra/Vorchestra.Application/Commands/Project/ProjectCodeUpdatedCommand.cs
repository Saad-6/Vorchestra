using MediatR;
using Shared.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;

namespace Vorchestra.Application.Commands.Project;

public class ProjectCodeUpdatedCommand : IRequest<ResponseModel<string>>
{
    public Guid ProjectId {  get; set; }
}

public class ProjectCodeUpdatedCommandHandler : IRequestHandler<ProjectCodeUpdatedCommand, ResponseModel<string>>
{
    private readonly IServerService _serverService;
    private readonly ITenantEventPublisher _tenantEventPublisher;
    private readonly ITenantService _tenantService;
    public ProjectCodeUpdatedCommandHandler(IServerService serverService, ITenantEventPublisher tenantEventPublisher, ITenantService tenantService)
    {
        _serverService = serverService;
        _tenantEventPublisher = tenantEventPublisher;
        _tenantService = tenantService;
    }
    public async Task<ResponseModel<string>> Handle(ProjectCodeUpdatedCommand request, CancellationToken cancellationToken)
    {
        // Implement the logic to handle the project code update here.
        // This might involve updating the project code in the database, triggering other processes, etc.
        // For demonstration purposes, we'll return a successful response with a placeholder message.
        return new ResponseModel<string>
        {
            Success = true,
            Data = "Project code updated successfully."
        };
    }
}
