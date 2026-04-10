using MediatR;
using Shared.Application.Models;
using Shared.Application.Vaidators;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Server;

public class UpdateServerCommand : UpdateServerDto, IRequest<ResponseModel<string>>
{
}
public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, ResponseModel<string>>
{
    private readonly IServerService _serverService;
    public UpdateServerCommandHandler(IServerService serverService)
    {
        _serverService = serverService;
    }
    public async Task<ResponseModel<string>> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<ServerStatus>(request.Status);

        return await _serverService.UpdateServerAsync(request, cancellationToken);
    }
}
