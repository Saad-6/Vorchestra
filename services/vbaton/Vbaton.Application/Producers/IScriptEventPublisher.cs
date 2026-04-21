using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;

namespace Vbaton.Application.Producers;

public interface IScriptEventPublisher
{
    Task<ResponseModel<List<ScriptsByIdsResponse>>> PublishScriptsByIdsEvent(ScriptsByIdsRequest request);
    Task<ResponseModel<List<ScriptsByIdsResponse>>> PublishScriptsByGroupIdsEvent(List<Guid> groupIds);
}
