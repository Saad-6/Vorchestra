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

public class ExecuteWorkFlowCommand : ExecutionRequestDto, IRequest<ResponseModel<string>>
{
}

public class ExecuteWorkFlowCommandHandler : IRequestHandler<ExecuteWorkFlowCommand, ResponseModel<string>>
{
    private static readonly Regex PlaceholderPattern = new(@"\{\{([^}]+)\}\}", RegexOptions.Compiled);

    private readonly ISshService _sshService;
    private readonly IScriptEventPublisher _scriptEventPublisher;
    private readonly IProjectEventPublisher _projectEventPublisher;
    private readonly ILogService _logService;

    public ExecuteWorkFlowCommandHandler(
        ISshService sshService,
        IScriptEventPublisher scriptEventPublisher,
        IProjectEventPublisher projectEventPublisher,
        ILogService logService)
    {
        _sshService = sshService;
        _scriptEventPublisher = scriptEventPublisher;
        _projectEventPublisher = projectEventPublisher;
        _logService = logService;
    }

    public async Task<ResponseModel<string>> Handle(ExecuteWorkFlowCommand request, CancellationToken cancellationToken)
    {
        var normalizedRequest = await MapToNormalizedExecutionRequest(request);

        var result = await _sshService.ExecuteCommandsAsync(normalizedRequest);

        var scriptOutputs = result.Data ?? [];
        var combinedOutput = string.Join("\n", scriptOutputs.Select(s => s.Output));

        await _logService.LogAsync(request, request.GroupIds, combinedOutput, result.Success, scriptOutputs);

        return new ResponseModel<string>
        {
            Success = result.Success,
            Message = result.Message,
            Data = combinedOutput
        };
    }

    private async Task<NormalizedExecutionRequest> MapToNormalizedExecutionRequest(ExecutionRequestDto request)
    {
        var response = request switch
        {
            _ when request.GroupIds != null && request.GroupIds.Count > 0
                => await _scriptEventPublisher.PublishScriptsByGroupIdsEvent(request.GroupIds),

            _ when request.ScriptIds != null && request.ScriptIds.Count > 0
                => await _scriptEventPublisher.PublishScriptsByIdsEvent(
                    new ScriptsByIdsRequest { ScriptIds = request.ScriptIds }),

            _ => throw new ArgumentException("Either GroupIds or ScriptIds must be provided.")
        };

        if (!response.Success)
            throw new InvalidOperationException($"Failed to fetch scripts: {response.Message}");

        var scripts = response?.Data?.OrderBy(m=>m.Order).SelectMany(m => m.Scripts).OrderBy(s => s.Order).ToList();

        await ResolveProjectAsync(request);

        SubstituteVariables(request, scripts);

        return new NormalizedExecutionRequest
        {
            Server = request.Server,
            Scripts = scripts,
        };
    }

    private async Task ResolveProjectAsync(ExecutionRequestDto request)
    {
        if(request?.Project?.Id != null && request?.Project?.Id != Guid.Empty)
        {
            var projectResponse = await _projectEventPublisher.PublishProjectByIdEventAsync(request.Project.Id);
            
            if (!projectResponse.Success)
            {
                throw new InvalidOperationException($"Project with Id not found: {projectResponse.Message}");
            }

            var projectContext = new ProjectContextDto
            {
                Id = request.Project.Id,
                IsSourceControl = request.Project.IsSourceControl,
                Source = request.Project.Source,
                Name = request.Project.Name,
                PersonalAccessToken = request.Project.PersonalAccessToken
            };

            request.Project = projectContext;
        }
    }
    private static void SubstituteVariables(ExecutionRequestDto request, List<ScriptResponse>? scripts)
    {
        if (scripts == null) return;

        // Resolve all sources once for the entire request — shared across all scripts.
        var sourceContext = VariableMap.BuildContext(request);

        foreach (var script in scripts)
        {
            if (script.Variables is not { Count: > 0 }) continue;

            // Build a name→value map for this script's declared variables.
            // name = the {{placeholder}} in script content, value = resolved from source.
            var nameContext = script.Variables
                .Where(v => sourceContext.ContainsKey(v.Source))
                .ToDictionary(v => v.Name, v => sourceContext[v.Source]);

            if (nameContext.Count == 0) continue;

            // Single regex pass per script — O(n) where n = script content length.
            // Undeclared or unresolvable placeholders are left unchanged.
            script.Content = PlaceholderPattern.Replace(script.Content, match =>
            {
                var name = match.Groups[1].Value;
                return nameContext.TryGetValue(name, out var value) ? value : match.Value;
            });
        }
    }
}
