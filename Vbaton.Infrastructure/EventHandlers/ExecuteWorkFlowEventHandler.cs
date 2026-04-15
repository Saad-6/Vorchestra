using MassTransit;
using MediatR;
using Shared.DTO;
using Vbaton.Application.Commands;

namespace Vbaton.Infrastructure.EventHandlers;

public class ExecuteWorkFlowEventHandler : IConsumer<ExecutionRequestCommand>
{
    private readonly IMediator _mediator;
    public ExecuteWorkFlowEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ExecutionRequestCommand> context)
    {
        var command = new ExecuteWorkFlowCommand
        {
            Server = context.Message.Server,
            Tenant = context.Message.Tenant,
            GroupId = context.Message.GroupId,
            ScriptIds = context.Message.ScriptIds,
            VariableContext = context.Message.VariableContext
        };

        var response = await _mediator.Send(command);

        await context.RespondAsync(response);
    }
}
