using Shared.Application.Models;

namespace Xcript.Application.Interfaces;

public interface IVariableSourceService
{
    Task<ResponseModel<IEnumerable<string>>> GetAvailableSourcesAsync();
}
