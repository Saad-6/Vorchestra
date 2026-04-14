using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xcript.Application.Commands.Variable;
using Xcript.Application.Queries.Variable;
using Xcript.Application.Queries.VariableSource;

namespace Xcript.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VariableController : ControllerBase
{
    private readonly IMediator _mediator;
    public VariableController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] VariableByIdQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] PaginatedVariableQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpGet("sources")]
    public async Task<IActionResult> GetSourcesAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AllVariableSourcesQuery(), cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateVariableCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateVariableCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync([FromQuery] DeleteVariableCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
