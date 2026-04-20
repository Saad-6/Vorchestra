using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Commands;

public class ChangeGroupOrderInWorkflowCommand : IRequest<ResponseModel<string>>
{
    public Guid WorkflowId { get; set; }
    public Guid GroupId { get; set; }
    public int OrderId { get; set; }
}
public class ChangeGroupOrderInWorkflowCommandHandler : IRequestHandler<ChangeGroupOrderInWorkflowCommand, ResponseModel<string>>
{
    private readonly IWorkflowGroupService _workflowService;
    public ChangeGroupOrderInWorkflowCommandHandler(IWorkflowGroupService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<string>> Handle(ChangeGroupOrderInWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.ChangeGroupOrderInWorkflowAsync(request.WorkflowId, request.GroupId, request.OrderId, cancellationToken);
    }
}
