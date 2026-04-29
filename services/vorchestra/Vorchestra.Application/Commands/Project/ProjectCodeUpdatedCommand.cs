using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Application.Models;
using Shared.Domain.Constants;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;

namespace Vorchestra.Application.Commands.Project;

public class ProjectCodeUpdatedCommand : IRequest<ResponseModel<string>>
{
    public Guid ProjectId { get; set; }
}

public class ProjectCodeUpdatedCommandHandler : IRequestHandler<ProjectCodeUpdatedCommand, ResponseModel<string>>
{
    private readonly ITenantProjectService _tenantProjectService;
    private readonly IServerService _serverService;
    private readonly ITenantEventPublisher _tenantEventPublisher;
    private readonly ILogger<ProjectCodeUpdatedCommandHandler> _logger;

    public ProjectCodeUpdatedCommandHandler(
        ITenantProjectService tenantProjectService,
        IServerService serverService,
        ITenantEventPublisher tenantEventPublisher,
        ILogger<ProjectCodeUpdatedCommandHandler> logger)
    {
        _tenantProjectService = tenantProjectService;
        _serverService = serverService;
        _tenantEventPublisher = tenantEventPublisher;
        _logger = logger;
    }

    public async Task<ResponseModel<string>> Handle(ProjectCodeUpdatedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Code update triggered for project {ProjectId}", request.ProjectId);

        // 1. Fetch workflows once — same trigger + project for all tenant instances
        var workflowsResponse = await _tenantEventPublisher.GetProjectWorkflowsAsync(
            WorkflowTrigger.Project.CODE_UPDATED, request.ProjectId);

        if (!workflowsResponse.Success)
        {
            _logger.LogError("Failed to fetch workflows for project {ProjectId}: {Message}",
                request.ProjectId, workflowsResponse.Message);
            return new ResponseModel<string> { Success = false, Message = workflowsResponse.Message };
        }

        var groupIds = workflowsResponse.Data?.Workflows?
            .OrderBy(w => w.Order)
            .SelectMany(w => w.GroupIds)
            .ToList();

        if (groupIds == null || groupIds.Count == 0)
        {
            _logger.LogWarning("No workflows configured for CODE_UPDATED on project {ProjectId} — nothing to publish", request.ProjectId);
            return new ResponseModel<string> { Success = true, Message = "No workflows configured for this trigger." };
        }

        // 2. Fetch all running tenant project instances for this project
        var tenantProjectsResponse = await _tenantProjectService
            .GetRunningTenantProjectsByProjectIdAsync(request.ProjectId, cancellationToken);

        if (!tenantProjectsResponse.Success)
        {
            _logger.LogError("Failed to fetch running tenant projects for project {ProjectId}: {Message}",
                request.ProjectId, tenantProjectsResponse.Message);
            return new ResponseModel<string> { Success = false, Message = tenantProjectsResponse.Message };
        }

        var tenantProjects = tenantProjectsResponse.Data!;

        if (tenantProjects.Count == 0)
        {
            _logger.LogInformation("No running tenant projects found for project {ProjectId} — nothing to notify", request.ProjectId);
            return new ResponseModel<string> { Success = true, Message = "No running tenant projects for this project." };
        }

        _logger.LogInformation("Found {Count} running tenant project(s) for project {ProjectId}",
            tenantProjects.Count, request.ProjectId);

        // 3. Group by server to avoid redundant server context fetches
        var serverGroups = tenantProjects
            .Where(tp => tp.ServerId.HasValue)
            .GroupBy(tp => tp.ServerId!.Value)
            .ToList();

        var skippedNoServer = tenantProjects.Count(tp => !tp.ServerId.HasValue);
        if (skippedNoServer > 0)
            _logger.LogWarning("{Count} tenant project(s) skipped — no server assigned", skippedNoServer);

        int successCount = 0;
        int failureCount = 0;

        // 4. Outer loop: per server
        foreach (var serverGroup in serverGroups)
        {
            var serverId = serverGroup.Key;

            var serverResponse = await _serverService.GetServerContextByIdAsync(serverId, cancellationToken);

            if (!serverResponse.Success)
            {
                _logger.LogError("Failed to fetch context for server {ServerId}: {Message} — skipping {Count} tenant(s)",
                    serverId, serverResponse.Message, serverGroup.Count());
                failureCount += serverGroup.Count();
                continue;
            }

            _logger.LogInformation("Publishing code update to {Count} tenant(s) on server {ServerId}",
                serverGroup.Count(), serverId);

            // 5. Inner loop: per tenant project on this server
            foreach (var tenantProject in serverGroup)
            {
                var tenantContextResponse = await _tenantProjectService
                    .GetTenantContextAsync(tenantProject.Id, cancellationToken);

                if (!tenantContextResponse.Success)
                {
                    _logger.LogError("Failed to fetch tenant context for TenantProject {TenantProjectId}: {Message}",
                        tenantProject.Id, tenantContextResponse.Message);
                    failureCount++;
                    continue;
                }

                var publishResponse = await _tenantEventPublisher.PublishEventAsync(
                    serverResponse.Data!, groupIds, tenantContextResponse.Data!);

                if (!publishResponse.Success)
                {
                    _logger.LogError("Failed to publish CODE_UPDATED event for TenantProject {TenantProjectId} on server {ServerId}: {Message}",
                        tenantProject.Id, serverId, publishResponse.Message);
                    failureCount++;
                }
                else
                {
                    _logger.LogInformation("Successfully published CODE_UPDATED for TenantProject {TenantProjectId} (Tenant {TenantId}) on server {ServerId}",
                        tenantProject.Id, tenantProject.TenantId, serverId);
                    successCount++;
                }
            }
        }

        _logger.LogInformation("Code update dispatch complete for project {ProjectId}: {SuccessCount} succeeded, {FailureCount} failed",
            request.ProjectId, successCount, failureCount);

        var message = failureCount == 0
            ? $"Code update dispatched to all {successCount} tenant(s) successfully."
            : $"Code update dispatched with partial failures: {successCount} succeeded, {failureCount} failed.";

        return new ResponseModel<string> { Success = true, Message = message };
    }
}
