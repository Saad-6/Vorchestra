using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Queries;

public class WorkflowByIdQuery : IRequest<ResponseModel<WorkflowDto>>   
{
    public Guid Id { get; set; }
}
public class WorkflowByIdQueryHandler : IRequestHandler<WorkflowByIdQuery, ResponseModel<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;
    public WorkflowByIdQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<WorkflowDto>> Handle(WorkflowByIdQuery request, CancellationToken cancellationToken)
    {
        return await _workflowService.GetWorkflowByIdAsync(request.Id, cancellationToken);
    }
}
