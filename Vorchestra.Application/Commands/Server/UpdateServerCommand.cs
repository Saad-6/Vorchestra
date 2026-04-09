using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.Application.Vaidators;
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
