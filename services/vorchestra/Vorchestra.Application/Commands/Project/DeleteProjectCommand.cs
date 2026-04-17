using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.Application.Commands.Project;

public class DeleteProjectCommand : IRequest<ResponseModel<string>>
{
    public Guid ProjectId { get; set; }
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ResponseModel<string>>
{
    private readonly IProjectService _projectService;
    public DeleteProjectCommandHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        return await _projectService.DeleteProjectAsync(request.ProjectId);
    }
}
