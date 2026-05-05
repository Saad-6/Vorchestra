using MassTransit;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Vbaton.Application.Producers;

namespace Vbaton.Infrastructure.EventPublishers;

public class ProjectEventPublisher : IProjectEventPublisher
{
    private readonly IRequestClient<ProjectByIdRequest> _requestClient;
    public ProjectEventPublisher(IRequestClient<ProjectByIdRequest> requestClient)
    {
        _requestClient = requestClient;
    }

    public async Task<ResponseModel<ProjectByIdResponse>> PublishProjectByIdEventAsync(Guid projectId)
    {
        var request = new ProjectByIdRequest { Id = projectId };
        var response = await _requestClient.GetResponse<ResponseModel<ProjectByIdResponse>>(request);
        return response.Message;
    }
}
