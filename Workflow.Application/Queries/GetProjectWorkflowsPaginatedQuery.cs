using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Queries;

public class GetProjectWorkflowsPaginatedQuery : IRequest<PaginatedResponseModel<WorkflowDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
}

public class GetProjectWorkflowsPaginatedQueryHandler : IRequestHandler<GetProjectWorkflowsPaginatedQuery, PaginatedResponseModel<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;

    public GetProjectWorkflowsPaginatedQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public async Task<PaginatedResponseModel<WorkflowDto>> Handle(GetProjectWorkflowsPaginatedQuery request, CancellationToken cancellationToken)
    {
        var filter = new FilterModel
        {
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Filters = new Dictionary<string, object>
            {
                { nameof(WorkflowDto.Name), request.SearchTerm ?? string.Empty }
            }
        };

        return await _workflowService.GetPaginatedProjectWorkflowsAsync(filter, cancellationToken);
    }
}
