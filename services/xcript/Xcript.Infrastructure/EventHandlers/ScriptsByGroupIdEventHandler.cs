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

        var queryResponse = await _mediator.Send(new GroupsByIdsQuery { GroupIds = request.GroupIds ?? new List<Guid>() });

        var response = new ResponseModel<List<ScriptsByIdsResponse>>
        {   
            Success = queryResponse.Success,
            Message = queryResponse.Message,
            Data = queryResponse.Data?.Select(m => new ScriptsByIdsResponse
            {
                GroupName = m.Name ?? string.Empty,
                Scripts = m.Scripts?.Select(s => new ScriptResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Content = s.Content,
                    Description = s.Description,
                    Order = s?.Order ?? 0,
                    Variables = s.Variables.Select(v => new ScriptVariableInfo
                    {
                        Name = v.Name,
                        Source = v.Source
                    }).ToList()
                }).ToList() ?? new List<ScriptResponse>()
            }).ToList() ?? new List<ScriptsByIdsResponse>()
        };

        await context.RespondAsync(response);
    }
}
