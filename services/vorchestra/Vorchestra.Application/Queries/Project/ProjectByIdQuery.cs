using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Project;

public class ProjectByIdQuery : IRequest<ResponseModel<ProjectViewDto>>
{
    public Guid Id { get; set; }
}
public class ProjectByIdQueryHandler : IRequestHandler<ProjectByIdQuery, ResponseModel<ProjectViewDto>>
{
    private readonly IProjectService _projectService;
    public ProjectByIdQueryHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<ResponseModel<ProjectViewDto>> Handle(ProjectByIdQuery request, CancellationToken cancellationToken)
    {
        return await _projectService.GetProjectByIdAsync(request.Id);
    }
}
