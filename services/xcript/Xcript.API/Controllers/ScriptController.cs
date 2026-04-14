using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xcript.Application.Commands.Script;
using Xcript.Application.Commands.ScriptVariable;
using Xcript.Application.Queries.Script;

namespace Xcript.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScriptController : ControllerBase
{
    private readonly IMediator _mediator;
    public ScriptController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] ScriptByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] PaginatedScriptQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateScriptCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateScriptCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync([FromQuery] DeleteScriptCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("variables")]
    public async Task<IActionResult> AssignVariableAsync([FromBody] AssignVariableToScriptCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("variables")]
    public async Task<IActionResult> RemoveVariableAsync([FromQuery] RemoveVariableFromScriptCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
