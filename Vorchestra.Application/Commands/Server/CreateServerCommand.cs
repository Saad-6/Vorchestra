using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.Application.Vaidators;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Server;

public class CreateServerCommand : CreateServerDto, IRequest<ResponseModel<string>> 
{
}
public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, ResponseModel<string>>
{
    private readonly IServerService _serverService;
    public CreateServerCommandHandler(IServerService serverService)
    {
        _serverService = serverService;
    }
    public async Task<ResponseModel<string>> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<ServerStatus>(request.Status);

        return await _serverService.CreateServerAsync(request, cancellationToken);
    }
}
