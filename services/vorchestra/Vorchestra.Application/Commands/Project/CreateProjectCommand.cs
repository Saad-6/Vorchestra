using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Project;

public class CreateProjectCommand : CreateProjectDto, IRequest<ResponseModel<Guid>>
{
    public string? ZipFilePath { get; set; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ResponseModel<Guid>>
{
    private readonly IProjectService _projectService;
    public CreateProjectCommandHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        return await _projectService.CreateProjectAsync(request);
    }
}
