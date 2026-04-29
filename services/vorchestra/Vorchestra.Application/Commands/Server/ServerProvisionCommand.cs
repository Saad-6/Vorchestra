using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Helpers;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;

namespace Vorchestra.Application.Commands.Server;

public class ServerProvisionCommand : IRequest<ResponseModel<string>>
{
    public Guid ServerId { get; set; }
    public Guid WorkflowId { get; set; }
}

public class ServerProvisionCommandHandler : IRequestHandler<ServerProvisionCommand, ResponseModel<string>>
{
    private readonly IServerEventPublisher _serverEventPublisher;
    private readonly IServerService _serverService;
    public ServerProvisionCommandHandler(IServerEventPublisher serverEventPublisher, IServerService serverService)
    {
        _serverEventPublisher = serverEventPublisher;
        _serverService = serverService;
    }
    public async Task<ResponseModel<string>> Handle(ServerProvisionCommand request, CancellationToken cancellationToken)
    {
        var serverContextResponse = await _serverService.GetServerContextByIdAsync(request.ServerId);

        if (!serverContextResponse.Success)
            return Utility.MapResponse(serverContextResponse);

        var workflowsResponse = await _serverEventPublisher.GetServerWorkflowByIdAsync(request.WorkflowId);

        if(!workflowsResponse.Success)
            return Utility.MapResponse(workflowsResponse);

        var groupIds = workflowsResponse.Data?.Workflows?.OrderBy(w => w.Order).SelectMany(w => w.GroupIds).ToList();

        var publishResponse = await _serverEventPublisher.PublishEventAsync(serverContextResponse.Data!, groupIds!);

        return publishResponse;

    }
}