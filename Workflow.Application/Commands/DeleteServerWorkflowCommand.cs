using MediatR;
using Shared.Application.Models;
using Workflow.Application.Interfaces;

namespace Workflow.Application.Commands;

public class DeleteServerWorkflowCommand : IRequest<ResponseModel<string>>
{
    public Guid Id { get; set; }
}

public class DeleteServerWorkflowCommandHandler : IRequestHandler<DeleteServerWorkflowCommand, ResponseModel<string>>
{
    private readonly IWorkflowService _workflowService;
    public DeleteServerWorkflowCommandHandler(IWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteServerWorkflowCommand request, CancellationToken cancellationToken)
    {
        return await _workflowService.DeleteServerWorkflowAsync(request.Id, cancellationToken);
    }
}