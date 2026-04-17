using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vorchestra.Application.Commands.Project;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Queries.Project;

namespace Vorchestra.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;

    public ProjectController(IMediator mediator, IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] GetAllProjectsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Post([FromForm] CreateProjectCommand command, IFormFile? zipFile, CancellationToken cancellationToken)
    {
        if (zipFile != null)
            command.ZipFilePath = await _fileStorageService.SaveFileAsync(zipFile.OpenReadStream(), zipFile.FileName);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Put([FromForm] UpdateProjectCommand command, IFormFile? zipFile, CancellationToken cancellationToken)
    {
        if (zipFile != null)
            command.ZipFilePath = await _fileStorageService.SaveFileAsync(zipFile.OpenReadStream(), zipFile.FileName);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
            return Ok(result);

        return BadRequest(result);
    }
}
