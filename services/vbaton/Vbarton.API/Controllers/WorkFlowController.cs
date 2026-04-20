using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vbaton.Application.Commands;

namespace Vbarton.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorkFlowController : ControllerBase
{
    private readonly IMediator _mediator;
    public WorkFlowController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost]
    public async Task<IActionResult> ExecuteWorkFlow([FromBody] ExecuteWorkFlowCommand request)
    {
        var response = await _mediator.Send(request);
        
        if (response.Success)
            return Ok(response);

        return BadRequest(response);
    }
}
