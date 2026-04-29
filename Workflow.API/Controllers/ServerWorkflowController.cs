using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workflow.Application.Commands;
using Workflow.Application.Queries;

namespace Workflow.API.Controllers;

[Route("api/server-workflow")]
[ApiController]
public class ServerWorkflowController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServerWorkflowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] GetServerWorkflowsPaginatedQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById([FromRoute] WorkflowByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateServerWorkflowCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdateServerWorkflowCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] DeleteServerWorkflowCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("groups")]
    public async Task<ActionResult> AssignGroup([FromBody] AssignGroupToWorkflowCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("groups")]
    public async Task<ActionResult> RemoveGroup([FromQuery] RemoveGroupFromWorkflowCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut("groups/order")]
    public async Task<ActionResult> ChangeGroupOrder([FromBody] ChangeGroupOrderInWorkflowCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
