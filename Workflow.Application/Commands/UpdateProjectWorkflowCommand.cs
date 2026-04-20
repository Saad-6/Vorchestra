
using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Commands;

public class UpdateProjectWorkflowCommand : UpdateProjectWorkflowDto, IRequest<ResponseModel<WorkflowDto>>
{
}
public class UpdateProjectWorkflowCommandHandler : IRequestHandler<UpdateProjectWorkflowCommand, ResponseModel<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;
    public UpdateProjectWorkflowCommandHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<WorkflowDto>> Handle(UpdateProjectWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.UpdateProjectWorkflowAsync(request, cancellationToken);
    }
}