using Shared.Application.Models;
using Shared.DTO;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface IServerService
{
    Task<PaginatedResponseModel<ServerViewDto>> GetPaginatedServersAsync(FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateServerAsync(CreateServerDto server, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> UpdateServerAsync(UpdateServerDto server, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteServerAsync(Guid serverId, CancellationToken cancellationToken = default);
    Task<ResponseModel<ServerContextDto>> GetServerContextByIdAsync(Guid serverId, CancellationToken cancellationToken = default);
}
