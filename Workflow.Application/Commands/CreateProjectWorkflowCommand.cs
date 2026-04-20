using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.DTO;

namespace Workflow.Application.Commands;

public class CreateProjectWorkflowCommand : CreateProjectWorkflowDto, IRequest<ResponseModel<Guid>>
{
}
public class CreateProjectWorkflowCommandHandler : IRequestHandler<CreateProjectWorkflowCommand, ResponseModel<Guid>>
{
    private readonly IWorkflowService _workflowService;
    public CreateProjectWorkflowCommandHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateProjectWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.CreateProjectWorkflowAsync(request, cancellationToken);
    }
}
