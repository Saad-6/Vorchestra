using MassTransit;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Vbaton.Application.Producers;

namespace Vbaton.Infrastructure.EventPublishers;

public class ScriptEventPublisher : IScriptEventPublisher
{
    private readonly IRequestClient<ScriptsByIdsRequest> _scriptsByIdsClient; 
    private readonly IRequestClient<ScriptsByGroupIdRequest> _scriptsByGroupIdClient;

    public ScriptEventPublisher(IRequestClient<ScriptsByIdsRequest> scriptsByIdsClient, IRequestClient<ScriptsByGroupIdRequest> scriptsByGroupIdClient)
    {
        _scriptsByIdsClient = scriptsByIdsClient;
        _scriptsByGroupIdClient = scriptsByGroupIdClient;
    }
    public async Task<ResponseModel<List<ScriptsByIdsResponse>>> PublishScriptsByGroupIdsEvent(List<Guid> groupIds)
    {
        var request = new ScriptsByGroupIdRequest { GroupIds = groupIds };
        var response = await _scriptsByGroupIdClient.GetResponse<ResponseModel<List<ScriptsByIdsResponse>>>(request);
        return response.Message;
    }

    public async Task<ResponseModel<List<ScriptsByIdsResponse>>> PublishScriptsByIdsEvent(ScriptsByIdsRequest request)
    {
        var response = await _scriptsByIdsClient.GetResponse<ResponseModel<List<ScriptsByIdsResponse>>>(request);
        return response.Message;
    }
}
