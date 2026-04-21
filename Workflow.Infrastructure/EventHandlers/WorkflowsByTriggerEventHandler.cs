using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Shared.DTO;
using Workflow.Application.Queries;

namespace Workflow.Infrastructure.EventHandlers;

public class WorkflowsByTriggerEventHandler : IConsumer<WorkflowsByTriggerRequest>
{
    private readonly IMediator _mediator;
    public async Task Consume(ConsumeContext<WorkflowsByTriggerRequest> context)
    {
        var message = context.Message;
        
        var queryResponse = await _mediator.Send(new WorkflowsByTriggerQuery { Trigger = message.Trigger });

        var response = new ResponseModel<WorkflowsByTriggerResponse>
        {
            Success = queryResponse.Success,
            Message = queryResponse.Message,
            Data = new WorkflowsByTriggerResponse 
            { 
                Workflows = queryResponse?.Data ?? [] 
            }
        };

        await context.RespondAsync(response);
    }
}
