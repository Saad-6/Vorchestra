using System.Text.RegularExpressions;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Shared.DTO;
using Vbaton.Application.Interfaces;
using Vbaton.Application.Models;
using Vbaton.Application.Producers;
using Vbaton.Application.Resolution;

namespace Vbaton.Application.Commands;

public class ExecuteWorkFlowCommand : ExecutionRequestCommand, IRequest<ResponseModel<string>>
{
}

public class ExecuteWorkFlowCommandHandler : IRequestHandler<ExecuteWorkFlowCommand, ResponseModel<string>>
{
    // Matches {{Tenant.Slug}}, {{Server.IpAddress}}, etc. — compiled once, shared across all requests.
    private static readonly Regex PlaceholderPattern = new(@"\{\{([^}]+)\}\}", RegexOptions.Compiled);

    private readonly ISshService _sshService;
    private readonly IScriptEventPublisher _scriptEventPublisher;
    private readonly ILogService _logService;

    public ExecuteWorkFlowCommandHandler(ISshService sshService, IScriptEventPublisher scriptEventPublisher, ILogService logService)
    {
        _sshService = sshService;
        _scriptEventPublisher = scriptEventPublisher;
        _logService = logService;
    }

    public async Task<ResponseModel<string>> Handle(ExecuteWorkFlowCommand request, CancellationToken cancellationToken)
    {
        var normalizedRequest = await MapToNormalizedExecutionRequest(request);

        var result = await _sshService.ExecuteCommandsAsync(normalizedRequest);

        var scriptOutputs = result.Data ?? [];
        var combinedOutput = string.Join("\n", scriptOutputs.Select(s => s.Output));

        await _logService.LogAsync(request, request.GroupId, combinedOutput, result.Success, scriptOutputs);

        return new ResponseModel<string>
        {
            Success = result.Success,
            Message = result.Message,
            Data = combinedOutput
        };
    }

    private async Task<NormalizedExecutionRequest> MapToNormalizedExecutionRequest(ExecutionRequestCommand request)
    {
        var response = request switch
        {
            _ when request.GroupId != null && request.GroupId != Guid.Empty
                => await _scriptEventPublisher.PublishScriptsByIdGroupEvent(request.GroupId.Value),

            _ when request.ScriptIds != null && request.ScriptIds.Count > 0
                => await _scriptEventPublisher.PublishScriptsByIdsEvent(
                    new ScriptsByIdsRequest { ScriptIds = request.ScriptIds }),

            _ => throw new ArgumentException("Either GroupId or ScriptIds must be provided.")
        };

        if (!response.Success)
            throw new InvalidOperationException($"Failed to fetch scripts: {response.Message}");

        var scripts = response?.Data?.Scripts;

        SubstituteVariables(request, scripts);

        return new NormalizedExecutionRequest
        {
            Server = request.Server,
            Scripts = scripts,
        };
    }

    private static void SubstituteVariables(ExecutionRequestCommand request, List<ScriptResponse>? scripts)
    {
        if (scripts == null) return;

        // Resolve all variables once for the entire request, not per script.
        var context = VariableMap.BuildContext(request);

        foreach (var script in scripts)
        {
            // Single regex pass per script — O(n) where n = script content length.
            // Unresolved placeholders (no matching source) are left as-is.
            script.Content = PlaceholderPattern.Replace(script.Content, match =>
            {
                var source = match.Groups[1].Value;
                return context.TryGetValue(source, out var value) ? value : match.Value;
            });
        }
    }
}
