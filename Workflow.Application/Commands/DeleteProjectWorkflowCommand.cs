using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Commands;

public class DeleteProjectWorkflowCommand : IRequest<ResponseModel<string>>
{
    public Guid Id { get; set; }
}
public class DeleteProjectWorkflowCommandHandler : IRequestHandler<DeleteProjectWorkflowCommand, ResponseModel<string>>
{
    private readonly IWorkflowService _workflowService;
    public DeleteProjectWorkflowCommandHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteProjectWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.DeleteProjectWorkflowAsync(request.Id, cancellationToken);
    }
}
