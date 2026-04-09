using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vorchestra.Application.Commands.Tenant;
using Vorchestra.Application.Queries.Tenant;

namespace Vorchestra.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly IMediator _mediator;
    public TenantController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery]TenantsPaginatedQuery query)
    {
        var result = await _mediator.Send(query);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateTenantCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdateTenantCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("apply-free-trial")]
    public async Task<ActionResult> ApplyFreeTrial([FromBody] TenantApplyFreeTrialCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("subscribe-plan")]
    public async Task<ActionResult> SubscribePlan([FromBody] TenantSubscribePlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
    [HttpPost("cancel-subscription")]
    public async Task<ActionResult> CancelSubscription([FromBody] CancelTenantPlanCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.Success)
            return Ok(result);
        return BadRequest(result);
    }
}