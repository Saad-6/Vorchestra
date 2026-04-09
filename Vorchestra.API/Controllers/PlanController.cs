using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vorchestra.Application.Commands.Plan;
using Vorchestra.Application.Queries.Plan;

namespace Vorchestra.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlanController : ControllerBase
{
    private readonly IMediator _mediator;
    public PlanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var result = await _mediator.Send(new GetAllPlansQuery());
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreatePlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(DeletePlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdatePlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}
