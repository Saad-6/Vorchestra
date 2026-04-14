using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vorchestra.Application.Commands.Server;
using Vorchestra.Application.Queries.Server;

namespace Vorchestra.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServerController : ControllerBase
{
    private readonly IMediator _mediator;
    public ServerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] ServersPaginatedQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateServerCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdateServerCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] DeleteServerCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
