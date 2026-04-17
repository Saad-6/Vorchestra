using MediatR;
using Shared.Application.Models;
using Shared.Application.Vaidators;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Server;

public class ServersPaginatedQuery : IRequest<ResponseModel<List<ServerViewDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
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

        var filterModel = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>()
            {
                { nameof(Domain.DataModels.Server.Status), request.Status },
                { nameof(Domain.DataModels.Server.Name), request.SearchTerm }
            }
        };
        return await _serverService.GetPaginatedServersAsync(filterModel, cancellationToken);
    }
}
