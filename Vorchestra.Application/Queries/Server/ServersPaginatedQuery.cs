using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.Application.Vaidators;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Server;

public class ServersPaginatedQuery : IRequest<ResponseModel<List<ServerViewDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Status { get; set; }
    public string? Name { get; set; }
}

public class ServersPaginatedQueryHandler : IRequestHandler<ServersPaginatedQuery, ResponseModel<List<ServerViewDto>>>
{
    private readonly IServerService _serverService;
    public ServersPaginatedQueryHandler(IServerService serverService)
    {
        _serverService = serverService;
    }
    public async Task<ResponseModel<List<ServerViewDto>>> Handle(ServersPaginatedQuery request, CancellationToken cancellationToken)
    {
        ConstantValidator.Validate<ServerStatus>(request.Status);

        return await _serverService.GetPaginatedServersAsync(request.PageNumber, request.PageSize, request.Status, request.Name, cancellationToken);
    }
}
