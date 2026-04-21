using Shared.Application.Models;
using Shared.DTO;
using Workflow.DTO;

namespace Workflow.Application.Interfaces;

public interface IWorkflowService
{
    Task<ResponseModel<WorkflowDto>> GetWorkflowByIdAsync(Guid workflowId, CancellationToken cancellationToken = default);
    Task<PaginatedResponseModel<WorkflowDto>> GetPaginatedWorkflowsAsync(FilterModel model, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteProjectWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);
    Task<ResponseModel<string>> DeleteServerWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateServerWorkflowAsync(CreateServerWorkflowDto createServerWorkflowDto, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreateProjectWorkflowAsync(CreateProjectWorkflowDto createProjectWorkflowDto, CancellationToken cancellationToken = default);
    Task<ResponseModel<WorkflowDto>> UpdateServerWorkflowAsync(UpdateServerWorkflowDto updateServerWorkflowDto, CancellationToken cancellationToken = default);
    Task<ResponseModel<WorkflowDto>> UpdateProjectWorkflowAsync(UpdateProjectWorkflowDto updateProjectWorkflowDto, CancellationToken cancellationToken = default);
    Task<ResponseModel<List<WorkflowContextDto>>> GetWorkFlowsByTriggerAsync(string workFlowTrigger);
    
}
