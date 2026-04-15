using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Xcript.Application.Queries.Group;

namespace Xcript.Infrastructure.EventHandlers;

public class ScriptsByGroupIdEventHandler : IConsumer<ScriptsByGroupIdRequest>
{
    private readonly IMediator _mediator;
    public ScriptsByGroupIdEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ScriptsByGroupIdRequest> context)
    {
        var request = context.Message;

        var queryResponse = await _mediator.Send(new GroupByIdQuery { Id = request.GroupId ?? Guid.Empty});

        var response = new ResponseModel<ScriptsByIdsResponse>
        {
            Success = queryResponse.Success,
            Message = queryResponse.Message,
            Data = new ScriptsByIdsResponse
            {
                GroupName = queryResponse.Data?.Name ?? string.Empty,
                Scripts = queryResponse.Data?.Scripts.Select(s => new ScriptResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Content = s.Content,
                    Description = s.Description,
                    Order = s?.Order ?? 0
                }).ToList() ?? new List<ScriptResponse>()
            }
        };

        await context.RespondAsync(response);
    }
}
