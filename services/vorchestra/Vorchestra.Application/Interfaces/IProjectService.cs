using Shared.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface IProjectService
{
    Task<PaginatedResponseModel<ProjectViewDto>> GetAllProjectsAsync(FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateProjectAsync(CreateProjectDto project);
    Task<ResponseModel<Guid>> UpdateProjectAsync(UpdateProjectDto project);
    Task<ResponseModel<string>> DeleteProjectAsync(Guid projectId);
}
