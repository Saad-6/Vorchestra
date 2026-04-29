using MediatR;
using Shared.Application.Models;
using Shared.DTO;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Queries;

public class WorkflowContextByIdQuery : IRequest<ResponseModel<WorkflowContextDto>>
{
    public Guid Id { get; set; }
}

public class WorkflowContextByIdQueryHandler : IRequestHandler<WorkflowContextByIdQuery, ResponseModel<WorkflowContextDto>>
{
    private readonly IWorkflowService _workflowService;

    public WorkflowContextByIdQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    public async Task<ResponseModel<WorkflowContextDto>> Handle(WorkflowContextByIdQuery request, CancellationToken cancellationToken)
    {
        return await _workflowService.GetWorkflowContextByIdAsync(request.Id, cancellationToken);
    }
}
