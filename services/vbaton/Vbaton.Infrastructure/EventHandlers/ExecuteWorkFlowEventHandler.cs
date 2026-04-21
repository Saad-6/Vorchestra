using MassTransit;
using MediatR;
using Shared.DTO;
using Vbaton.Application.Commands;

namespace Vbaton.Infrastructure.EventHandlers;

public class ExecuteWorkFlowEventHandler : IConsumer<ExecutionRequestDto>
{
    private readonly IMediator _mediator;
    public ExecuteWorkFlowEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ExecutionRequestDto> context)
    {
        var command = new ExecuteWorkFlowCommand
        {
            Server = context.Message.Server,
            Tenant = context.Message.Tenant,
            GroupIds = context.Message.GroupIds,
            ScriptIds = context.Message.ScriptIds,
        };

        var response = await _mediator.Send(command);

        await context.RespondAsync(response);
    }
}
