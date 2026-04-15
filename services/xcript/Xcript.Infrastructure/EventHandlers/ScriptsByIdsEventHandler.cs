using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Xcript.Application.Queries.Script;

namespace Xcript.Infrastructure.EventHandlers;

public class ScriptsByIdsEventHandler : IConsumer<ScriptsByIdsRequest>
{
    private readonly IMediator _mediator;
    public ScriptsByIdsEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ScriptsByIdsRequest> context)
    {
        var request = context.Message;

        var queryResponse = await _mediator.Send(new ScriptsByIdsQuery {ScriptIds = request.ScriptIds }); 

        var response = new ResponseModel<ScriptsByIdsResponse>
        {
            Success = queryResponse.Success,
            Message = queryResponse.Message,
            Data = queryResponse.Data != null ? new ScriptsByIdsResponse
            {
                Scripts = queryResponse.Data.Select(s => new ScriptResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Content = s.Content,
                    Description = s.Description,
                }).ToList()
            } : null
        };

        await context.RespondAsync(response);

    }
}
