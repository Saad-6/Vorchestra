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
    public async Task<ActionResult> Get([FromQuery] TenantsPaginatedQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdateTenantCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("apply-free-trial")]
    public async Task<ActionResult> ApplyFreeTrial([FromBody] TenantApplyFreeTrialCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("subscribe-plan")]
    public async Task<ActionResult> SubscribePlan([FromBody] TenantSubscribePlanCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost("cancel-subscription")]
    public async Task<ActionResult> CancelSubscription([FromBody] CancelTenantPlanCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
