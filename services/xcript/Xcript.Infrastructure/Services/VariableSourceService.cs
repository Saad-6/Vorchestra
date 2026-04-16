using Shared.Application.Models;
using Shared.Domain.Constants;
using System.Reflection;
using Xcript.Application.Interfaces;

namespace Xcript.Infrastructure.Services;

public class VariableSourceService : IVariableSourceService
{

    public Task<ResponseModel<IEnumerable<string>>> GetAvailableSourcesAsync()
    {
        var sources = typeof(ScriptVariableSource.Tenant)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly)
            .Select(f => (string)f.GetRawConstantValue())
            .Concat(
                typeof(ScriptVariableSource.Server)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly)
                    .Select(f => (string)f.GetRawConstantValue())
            );
        var response = new ResponseModel<IEnumerable<string>>
        {
            Success = true,
            Message = "Available variable sources retrieved successfully.",
            Data = sources
        };

        return Task.FromResult(response);
    }
}
