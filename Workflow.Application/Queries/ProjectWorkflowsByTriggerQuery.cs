using MediatR;
using Shared.Application.Models;
using Shared.DTO;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Queries;

public class ProjectWorkflowsByTriggerQuery : IRequest<ResponseModel<List<WorkflowContextDto>>>
{
    public string Trigger { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
}

public class WorkflowsByTriggerQueryHandler : IRequestHandler<ProjectWorkflowsByTriggerQuery, ResponseModel<List<WorkflowContextDto>>>
{
    private readonly IWorkflowService _workflowService;
    public WorkflowsByTriggerQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<List<WorkflowContextDto>>> Handle(ProjectWorkflowsByTriggerQuery request, CancellationToken cancellationToken)
    {

        return await _workflowService.GetProjectWorkFlowsByTriggerAsync(request.Trigger, request.ProjectId);
    }
}