using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;

namespace Vorchestra.Application.Commands.Server;

public class DeleteServerCommand : IRequest<ResponseModel<string>>
{
    public Guid ServerId { get; set; }
}
public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, ResponseModel<string>>
{
    private readonly IServerService _serverService;
    public DeleteServerCommandHandler(IServerService serverService)
    {
        _serverService = serverService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        return await _serverService.DeleteServerAsync(request.ServerId, cancellationToken);
    }
}