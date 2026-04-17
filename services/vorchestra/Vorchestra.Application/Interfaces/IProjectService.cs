using Shared.Application.Models;
using Vorchestra.Application.Commands.Project;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface IProjectService
{
    Task<PaginatedResponseModel<ProjectViewDto>> GetAllProjectsAsync(FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateProjectAsync(CreateProjectCommand project);
    Task<ResponseModel<Guid>> UpdateProjectAsync(UpdateProjectCommand project);
    Task<ResponseModel<string>> DeleteProjectAsync(Guid projectId);
}
