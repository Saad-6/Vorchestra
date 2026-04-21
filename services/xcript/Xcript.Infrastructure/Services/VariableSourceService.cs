using Shared.Application.Models;
using Shared.Domain.Constants;
using System.Reflection;
using Xcript.Application.Interfaces;

namespace Xcript.Infrastructure.Services;

public class VariableSourceService : IVariableSourceService
{

    public Task<ResponseModel<IEnumerable<string>>> GetAvailableSourcesAsync(bool fetchTenantVariables = true)
    {
        var sources = typeof(ScriptVariableSource.Server)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly)
                    .Select(f => (string)f.GetRawConstantValue());

        if (fetchTenantVariables)
        {
            sources.Concat(
                typeof(ScriptVariableSource.Tenant)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly)
                    .Select(f => (string)f.GetRawConstantValue())
            );
        }
        var response = new ResponseModel<IEnumerable<string>>
        {
            Success = true,
            Message = "Available variable sources retrieved successfully.",
            Data = sources
        };

        return Task.FromResult(response);
    }
}
