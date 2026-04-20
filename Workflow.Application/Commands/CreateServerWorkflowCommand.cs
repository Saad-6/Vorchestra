using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Commands;

public class CreateServerWorkflowCommand : CreateServerWorkflowDto, IRequest<ResponseModel<Guid>>
{
}
public class CreateServerWorkflowCommandHandler : IRequestHandler<CreateServerWorkflowCommand, ResponseModel<Guid>>
{
    private readonly IWorkflowService _workflowService;
    public CreateServerWorkflowCommandHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateServerWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.CreateServerWorkflowAsync(request, cancellationToken);
    }
}

