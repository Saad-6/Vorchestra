using MediatR;
using Shared.Application.Models;
using Shared.DTO;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Queries;

public class ServerWorkflowsByTriggerQuery : IRequest<ResponseModel<List<WorkflowContextDto>>>
{
    public string Trigger { get; set; } = string.Empty;
    public Guid ServerId { get; set; }
}

public class ServerWorkflowsByTriggerQueryHandler : IRequestHandler<ServerWorkflowsByTriggerQuery, ResponseModel<List<WorkflowContextDto>>>
{
    private readonly IWorkflowService _workflowService;
    public ServerWorkflowsByTriggerQueryHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<List<WorkflowContextDto>>> Handle(ServerWorkflowsByTriggerQuery request, CancellationToken cancellationToken)
    {

        return await _workflowService.GetServerWorkFlowsByTriggerAsync(request.Trigger, request.ServerId);
    }
}