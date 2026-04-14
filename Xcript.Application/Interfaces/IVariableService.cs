using Shared.Application.Models;
using Xcript.Application.Queries.Variable;
using Xcript.Domain.DataModels;
using Xcript.DTOs;

namespace Xcript.Application.Interfaces;

public interface IVariableService
{
    Task<ResponseModel<Guid>> CreateAsync(CreateVariableDto dto, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> UpdateAsync(Guid id, UpdateVariableDto dto, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResponseModel<VariableViewDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResponseModel<VariableViewDto>> GetPaginatedVariablesAsync(FilterModel filter, CancellationToken cancellationToken = default);
}
