using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;

namespace Vbaton.Application.Producers;

public interface IScriptEventPublisher
{
    Task<ResponseModel<ScriptsByIdsResponse>> PublishScriptsByIdsEvent(ScriptsByIdsRequest request);
    Task<ResponseModel<ScriptsByIdsResponse>> PublishScriptsByIdGroupEvent(Guid groupId);
}
