using Shared.Domain.Constants;
using Shared.DTO;

namespace Vbaton.Application.Resolution;

/// <summary>
/// Single source of truth for variable resolution.
/// Maps each ScriptVariableSource constant to the field it reads from the execution request.
/// To add a new variable source: add one entry here — nowhere else needs to change.
/// </summary>
public static class VariableMap
{
    private static readonly IReadOnlyDictionary<string, Func<ExecutionRequestCommand, string?>> Resolvers =
        new Dictionary<string, Func<ExecutionRequestCommand, string?>>
        {
            // Tenant sources
            [ScriptVariableSource.Tenant.ID]                = r => r.Tenant?.Id.ToString(),
            [ScriptVariableSource.Tenant.NAME]              = r => r.Tenant?.Name,
            [ScriptVariableSource.Tenant.SLUG]              = r => r.Tenant?.Slug,
            [ScriptVariableSource.Tenant.PORT]              = r => r.Tenant?.Port.ToString(),
            [ScriptVariableSource.Tenant.DOMAIN]            = r => r.Tenant?.Domain,
            [ScriptVariableSource.Tenant.CONNECTION_STRING] = r => r.Tenant?.ConnectionString,

            // Server sources
            [ScriptVariableSource.Server.ID]                = r => r.Server?.Id.ToString(),
            [ScriptVariableSource.Server.IP_ADDRESS]        = r => r.Server?.IpAddress,
            [ScriptVariableSource.Server.PORT]              = r => r.Server?.Port.ToString(),
            [ScriptVariableSource.Server.USERNAME]          = r => r.Server?.Username,
            [ScriptVariableSource.Server.PASSWORD]          = r => r.Server?.Password,
        };

    /// <summary>
    /// Resolves all available variables from the request into a flat dictionary.
    /// Keys are the source strings (e.g. "Tenant.Slug"), values are the resolved strings.
    /// Null values are omitted — unresolvable placeholders are left as-is in script content.
    /// </summary>
    public static Dictionary<string, string> BuildContext(ExecutionRequestCommand request)
    {
        var context = new Dictionary<string, string>(Resolvers.Count);

        foreach (var (source, resolve) in Resolvers)
        {
            var value = resolve(request);
            if (value is not null)
                context[source] = value;
        }

        return context;
    }
}
