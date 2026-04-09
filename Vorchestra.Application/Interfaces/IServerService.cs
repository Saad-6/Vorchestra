using Vorchestra.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface IServerService
{
    Task<PaginatedResponseModel<ServerViewDto>> GetPaginatedServersAsync(int pageNumber, int pageSize, string? status = null, string? query = null, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> CreateServerAsync(CreateServerDto server, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> UpdateServerAsync(UpdateServerDto server, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteServerAsync(Guid serverId, CancellationToken cancellationToken = default);
}
