using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Commands;

public class AssignGroupToWorkflowCommand : IRequest<ResponseModel<string>>
{
    public Guid WorkflowId { get; set; }
    public Guid GroupId { get; set; }
}
public class AssignGroupToWorkflowCommandHandler : IRequestHandler<AssignGroupToWorkflowCommand, ResponseModel<string>>
{
    private readonly IWorkflowGroupService _workflowService;
    public AssignGroupToWorkflowCommandHandler(IWorkflowGroupService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<string>> Handle(AssignGroupToWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.AddGroupToWorkflowAsync(request.WorkflowId, request.GroupId, cancellationToken);
    }
}