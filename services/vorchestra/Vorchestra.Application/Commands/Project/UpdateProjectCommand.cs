using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Project;

public class UpdateProjectCommand : UpdateProjectDto, IRequest<ResponseModel<Guid>>
{
    public string? ZipFilePath { get; set; }
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ResponseModel<Guid>>
{
    private readonly IProjectService _projectService;
    public UpdateProjectCommandHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }
    public async Task<ResponseModel<Guid>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        return await _projectService.UpdateProjectAsync(request);
    }
}
