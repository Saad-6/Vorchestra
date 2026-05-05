using Shared.Application.Models;
using Shared.Contracts.ResponseModels;

namespace Vbaton.Application.Producers;

public interface IProjectEventPublisher
{
    Task<ResponseModel<ProjectByIdResponse>> PublishProjectByIdEventAsync(Guid projectId);
}
