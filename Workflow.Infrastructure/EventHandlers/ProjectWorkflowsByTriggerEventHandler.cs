using MassTransit;
using MediatR;
using Shared.Application.Models;
using Shared.Contracts.RequestModels;
using Shared.Contracts.ResponseModels;
using Workflow.Application.Queries;

namespace Workflow.Infrastructure.EventHandlers;

public class ProjectWorkflowsByTriggerEventHandler : MassTransit.IConsumer<ProjectWorkflowsByTriggerRequest>
{
    private readonly IMediator _mediator;

    public ProjectWorkflowsByTriggerEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ProjectWorkflowsByTriggerRequest> context)
    {
        var message = context.Message;
        
        var queryResponse = await _mediator.Send(new ProjectWorkflowsByTriggerQuery { Trigger = message.Trigger, ProjectId = message.ProjectId });

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
