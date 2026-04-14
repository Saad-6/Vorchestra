using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xcript.Application.Commands.Group;
using Xcript.Application.Commands.GroupScript;
using Xcript.Application.Queries.Group;

namespace Xcript.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly IMediator _mediator;
    public GroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] GroupByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] PaginatedGroupQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync([FromQuery] DeleteGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("scripts")]
    public async Task<IActionResult> AddScriptAsync([FromBody] AddScriptToGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("scripts")]
    public async Task<IActionResult> RemoveScriptAsync([FromQuery] RemoveScriptFromGroupCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
