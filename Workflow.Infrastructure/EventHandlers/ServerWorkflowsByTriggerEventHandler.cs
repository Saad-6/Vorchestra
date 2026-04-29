using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Workflow.Application.Queries;

namespace Workflow.Infrastructure.EventHandlers;

public class ServerWorkflowsByTriggerEventHandler : MassTransit.IConsumer<ServerWorkflowsByTriggerRequest>
{
    private readonly IMediator _mediator;

    public ServerWorkflowsByTriggerEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ServerWorkflowsByTriggerRequest> context)
    {
        var message = context.Message;

        var queryResponse = await _mediator.Send(new ServerWorkflowsByTriggerQuery { Trigger = message.Trigger, ServerId = message.ServerId });

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
