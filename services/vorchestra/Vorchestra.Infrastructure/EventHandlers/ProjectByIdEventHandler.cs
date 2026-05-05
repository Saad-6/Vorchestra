using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Vorchestra.Application.Queries.Project;

namespace Vorchestra.Infrastructure.EventHandlers;

public class ProjectByIdEventHandler : IConsumer<ProjectByIdRequest>
{
    private readonly IMediator _mediator;
    public ProjectByIdEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ProjectByIdRequest> context)
    {
        var queryResponse = await _mediator.Send(new ProjectByIdQuery { Id = context.Message.Id });
        var data = queryResponse.Data;
        var response = new ResponseModel<ProjectByIdResponse>
        {
            Success = queryResponse.Success,
            Message = queryResponse.Message,
            Data = new ProjectByIdResponse
            {
                IsSourceControl = !string.IsNullOrWhiteSpace(data?.Url),
                PersonalAccessToken = data?.PersonalAccessToken,
                Id = context.Message.Id,
                Name = data?.Name,
                Source = string.IsNullOrWhiteSpace(data?.Branch) ? data?.ZipFilePath : data?.Url
            }
        };
        await context.RespondAsync(response);
    }
}
