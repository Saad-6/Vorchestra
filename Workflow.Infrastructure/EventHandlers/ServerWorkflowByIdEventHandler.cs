using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Workflow.Application.Queries;

namespace Workflow.Infrastructure.EventHandlers;

public class ServerWorkflowByIdEventHandler : MassTransit.IConsumer<ServerWorkflowByIdRequest>
{
    private readonly IMediator _mediator;

    public ServerWorkflowByIdEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ServerWorkflowByIdRequest> context)
    {
        var queryResponse = await _mediator.Send(new WorkflowContextByIdQuery { Id = context.Message.WorkflowId });

        var response = new ResponseModel<WorkflowsByTriggerResponse>
        {
            Success = queryResponse.Success,
            Message = queryResponse.Message,
            Data = new WorkflowsByTriggerResponse
            {
                Workflows = queryResponse.Data != null ? [queryResponse.Data] : []
            }
        };

        await context.RespondAsync(response);
    }
}
