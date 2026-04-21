using MediatR;
using Shared.Application.Models;
using Shared.DTO;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Queries;

public class WorkflowsByTriggerQuery : IRequest<ResponseModel<List<WorkflowContextDto>>>
{
    public string Trigger { get; set; } = string.Empty;
}

public class WorkflowsByTriggerQueryHandler : IRequestHandler<WorkflowsByTriggerQuery, ResponseModel<List<WorkflowContextDto>>>
{
    private readonly IWorkflowService _workflowService;
    public WorkflowsByTriggerQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<List<WorkflowContextDto>>> Handle(WorkflowsByTriggerQuery request, CancellationToken cancellationToken)
    {

        return await _workflowService.GetWorkFlowsByTriggerAsync(request.Trigger);
    }
}