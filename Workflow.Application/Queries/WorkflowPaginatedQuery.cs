using MediatR;
using Shared.Application.Models;
using Shared.DTO;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Queries;

public class WorkflowPaginatedQuery : IRequest<PaginatedResponseModel<WorkflowDto>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchTerm { get; set; }
}
public class WorkflowPaginatedQueryHandler : IRequestHandler<WorkflowPaginatedQuery, PaginatedResponseModel<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;
    public WorkflowPaginatedQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<PaginatedResponseModel<WorkflowDto>> Handle(WorkflowPaginatedQuery request, CancellationToken cancellationToken)
    {
        var filterModel = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>
            {
                { nameof(BaseWorkflowDto.Name), request.SearchTerm ?? string.Empty }
            }
        };
        return await _workflowService.GetPaginatedWorkflowsAsync(filterModel, cancellationToken);
    }
}