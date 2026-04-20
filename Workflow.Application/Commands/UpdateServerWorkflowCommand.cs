using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Commands;

public class UpdateServerWorkflowCommand : UpdateServerWorkflowDto, IRequest<ResponseModel<WorkflowDto>>
{
}
public class UpdateServerWorkflowCommandHandler : IRequestHandler<UpdateServerWorkflowCommand, ResponseModel<WorkflowDto>>
{
    private readonly IWorkflowService _workflowService;
    public UpdateServerWorkflowCommandHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<WorkflowDto>> Handle(UpdateServerWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.UpdateServerWorkflowAsync(request, cancellationToken);
    }
}
