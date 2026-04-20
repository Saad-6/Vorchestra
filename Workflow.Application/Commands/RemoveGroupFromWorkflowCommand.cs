using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Commands;

public class RemoveGroupFromWorkflowCommand : IRequest<ResponseModel<string>>
{
    public Guid WorkflowId { get; set; }
    public Guid GroupId { get; set; }
}

public class RemoveGroupFromWorkflowCommandHandler : IRequestHandler<RemoveGroupFromWorkflowCommand, ResponseModel<string>>
{
    private readonly IWorkflowGroupService _workflowService;
    public RemoveGroupFromWorkflowCommandHandler(IWorkflowGroupService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<string>> Handle(RemoveGroupFromWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.RemoveGroupFromWorkflowAsync(request.WorkflowId, request.GroupId, cancellationToken);
    }
}