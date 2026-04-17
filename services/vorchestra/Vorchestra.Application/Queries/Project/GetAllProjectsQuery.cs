using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Project;

public class GetAllProjectsQuery : IRequest<PaginatedResponseModel<ProjectViewDto>>
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PaginatedResponseModel<ProjectViewDto>>
{
    private readonly IProjectService _projectService;
    public GetAllProjectsQueryHandler(IProjectService projectService)
    {
        _projectService = projectService;
    }
    public async Task<PaginatedResponseModel<ProjectViewDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var filterModel = new FilterModel
        {
            Page = request.Page,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>
            {
                { nameof(Domain.DataModels.Project.Name), request.SearchTerm }
            }
        };
        return await _projectService.GetAllProjectsAsync(filterModel, cancellationToken);
    }
}
