using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Shared.DTO;
using Workflow.Application.Interfaces;
using Workflow.Domain.DataModels;
using Workflow.DTO;

namespace Workflow.Infrastructure.Services;

public class WorkflowService : IWorkflowService
{
    private readonly WorkflowDbContext _context;
    public WorkflowService(WorkflowDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<Guid>> CreateProjectWorkflowAsync(CreateProjectWorkflowDto createProjectWorkflowDto, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.ProjectWorkflows.AnyAsync(x => x.Name == createProjectWorkflowDto.Name && x.ProjectId == createProjectWorkflowDto.ProjectId, cancellationToken);
        if(nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A workflow with the same name already exists for this project.",
                Data = Guid.Empty
            };
        }

        var createdWorkflow = await CreateWorkFlow(createProjectWorkflowDto, projectId: createProjectWorkflowDto.ProjectId, cancellationToken: cancellationToken);

        await _context.SaveChangesAsync();

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Project workflow created successfully.",
            Data = createdWorkflow
        };
    }

    public async Task<ResponseModel<Guid>> CreateServerWorkflowAsync(CreateServerWorkflowDto createServerWorkflowDto, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.ServerWorkflows.AnyAsync(x => x.Name == createServerWorkflowDto.Name && x.ServerId == createServerWorkflowDto.ServerId, cancellationToken);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A workflow with the same name already exists for this project.",
                Data = Guid.Empty
            };
        }

        var createdWorkflow = await CreateWorkFlow(createServerWorkflowDto, serverId: createServerWorkflowDto.ServerId, cancellationToken: cancellationToken);

        await _context.SaveChangesAsync();

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Project workflow created successfully.",
            Data = createdWorkflow
        };
    }

    async Task<Guid> CreateWorkFlow(BaseWorkflowDto workFlow, Guid? projectId = null, Guid? serverId = null, CancellationToken cancellationToken = default)
    {
        if(projectId.HasValue)
        {
            var workflow = new ProjectWorkflow
            {
                ProjectId = projectId.Value,
                Id = Guid.NewGuid(),
                Name = workFlow.Name,
                Description = workFlow.Description,
                IsActive = workFlow.IsActive,
                Trigger = workFlow.Trigger,
            };
            await _context.AddAsync(workflow, cancellationToken);
            return workflow.Id;
        }
        else if(serverId.HasValue)
        {
            var workflow = new ServerWorkflow
            {
                ServerId = serverId.Value,
                Id = Guid.NewGuid(),
                Name = workFlow.Name,
                Description = workFlow.Description,
                IsActive = workFlow.IsActive,
                Trigger = workFlow.Trigger,
            };
            await _context.AddAsync(workflow, cancellationToken);
            return workflow.Id;
        }
        else
        {
            throw new ArgumentException("Either projectId or serverId must be provided.");
        }
    }
    public async Task<ResponseModel<string>> DeleteProjectWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await _context.ProjectWorkflows.FindAsync(new object[] { workflowId }, cancellationToken);

        if(workflow == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Workflow not found.",
                Data = null
            };
        }

        _context.ProjectWorkflows.Remove(workflow);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Workflow deleted successfully.",
            Data = null
        };
    }

    public async Task<ResponseModel<string>> DeleteServerWorkflowAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await _context.ServerWorkflows.FindAsync(new object[] { workflowId }, cancellationToken);

        if (workflow == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Workflow not found.",
                Data = null
            };
        }

        _context.ServerWorkflows.Remove(workflow);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Workflow deleted successfully.",
            Data = null
        };
    }

    public async Task<PaginatedResponseModel<WorkflowDto>> GetPaginatedWorkflowsAsync(FilterModel model, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectWorkflows.AsNoTracking().Select(x => new WorkflowDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
        }).Union(_context.ServerWorkflows.AsNoTracking().Select(x => new WorkflowDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
        }));

        query = model.ApplyFilters(query);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query.ToListAsync(cancellationToken);

        return new PaginatedResponseModel<WorkflowDto>
        {
            Success = true,
            Message = "Workflows retrieved successfully.",
            Data = items,
            TotalCount = count
        };

    }

    public async Task<PaginatedResponseModel<WorkflowDto>> GetPaginatedProjectWorkflowsAsync(FilterModel model, CancellationToken cancellationToken = default)
    {
        var query = _context.ProjectWorkflows.AsNoTracking().Select(x => new WorkflowDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
            ProjectId = x.ProjectId
        });

        query = model.ApplyFilters(query);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.ToListAsync(cancellationToken);

        return new PaginatedResponseModel<WorkflowDto>
        {
            Success = true,
            Message = "Project workflows retrieved successfully.",
            Data = items,
            TotalCount = count
        };
    }

    public async Task<PaginatedResponseModel<WorkflowDto>> GetPaginatedServerWorkflowsAsync(FilterModel model, CancellationToken cancellationToken = default)
    {
        var query = _context.ServerWorkflows.AsNoTracking().Select(x => new WorkflowDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
            ServerId = x.ServerId
        });

        //query = model.ApplyFilters(query);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.ToListAsync(cancellationToken);

        return new PaginatedResponseModel<WorkflowDto>
        {
            Success = true,
            Message = "Server workflows retrieved successfully.",
            Data = items,
            TotalCount = count
        };
    }

    public async Task<ResponseModel<WorkflowDto>> GetWorkflowByIdAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await _context.ProjectWorkflows.AsNoTracking().Where(x => x.Id == workflowId).Select(x => new WorkflowDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
        }).FirstOrDefaultAsync(cancellationToken);

        if(workflow != null)
        {
            return new ResponseModel<WorkflowDto>
            {
                Success = true,
                Message = "Workflow retrieved successfully.",
                Data = workflow
            };
        }
        workflow = await _context.ServerWorkflows.AsNoTracking().Where(x => x.Id == workflowId).Select(x => new WorkflowDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
        }).FirstOrDefaultAsync(cancellationToken);

        return new ResponseModel<WorkflowDto>
        {
            Success = workflow != null,
            Message = workflow != null ? "Workflow retrieved successfully." : "Workflow not found.",
            Data = workflow
        };

    }

    public async Task<ResponseModel<WorkflowContextDto>> GetWorkflowContextByIdAsync(Guid workflowId, CancellationToken cancellationToken = default)
    {
        var workflow = await _context.ProjectWorkflows
            .AsNoTracking()
            .Where(x => x.Id == workflowId)
            .Select(x => new WorkflowContextDto
            {
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                Trigger = x.Trigger,
                GroupIds = _context.WorkflowGroups.Where(g => g.WorkflowId == x.Id).Select(g => g.GroupId).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (workflow == null)
        {
            workflow = await _context.ServerWorkflows
                .AsNoTracking()
                .Where(x => x.Id == workflowId)
                .Select(x => new WorkflowContextDto
                {
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Trigger = x.Trigger,
                    GroupIds = _context.WorkflowGroups.Where(g => g.WorkflowId == x.Id).Select(g => g.GroupId).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new ResponseModel<WorkflowContextDto>
        {
            Success = workflow != null,
            Message = workflow != null ? "Workflow retrieved successfully." : "Workflow not found.",
            Data = workflow
        };
    }

    public async Task<ResponseModel<WorkflowDto>> UpdateProjectWorkflowAsync(UpdateProjectWorkflowDto updateProjectWorkflowDto, CancellationToken cancellationToken = default)
    {
        var workflow = await _context.ProjectWorkflows.FindAsync(new object[] { updateProjectWorkflowDto.Id }, cancellationToken);
        if(workflow == null)
        {
            return new ResponseModel<WorkflowDto>
            {
                Success = false,
                Message = "Workflow not found.",
                Data = null
            };
        }
        workflow.Name = updateProjectWorkflowDto.Name;
        workflow.Description = updateProjectWorkflowDto.Description;
        workflow.IsActive = updateProjectWorkflowDto.IsActive;
        workflow.UpdatedAt = DateTime.UtcNow;
        workflow.Trigger = updateProjectWorkflowDto.Trigger;

        _context.Update(workflow);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<WorkflowDto>
        {
            Success = true,
            Message = "Workflow updated successfully.",
            Data = new WorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                IsActive = workflow.IsActive,
                Trigger = workflow.Trigger,
            }
        };
    }

    public async Task<ResponseModel<WorkflowDto>> UpdateServerWorkflowAsync(UpdateServerWorkflowDto updateServerWorkflowDto, CancellationToken cancellationToken = default)
    {
        var workflow = await _context.ServerWorkflows.FindAsync(new object[] { updateServerWorkflowDto.Id }, cancellationToken);
        if (workflow == null)
        {
            return new ResponseModel<WorkflowDto>
            {
                Success = false,
                Message = "Workflow not found.",
                Data = null
            };
        }
        workflow.Name = updateServerWorkflowDto.Name;
        workflow.Description = updateServerWorkflowDto.Description;
        workflow.IsActive = updateServerWorkflowDto.IsActive;
        workflow.UpdatedAt = DateTime.UtcNow;
        workflow.Trigger = updateServerWorkflowDto.Trigger;

        _context.Update(workflow);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<WorkflowDto>
        {
            Success = true,
            Message = "Workflow updated successfully.",
            Data = new WorkflowDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                IsActive = workflow.IsActive,
                Trigger = workflow.Trigger,
            }
        };
    }

    public async Task<ResponseModel<List<WorkflowContextDto>>> GetWorkFlowsByTriggerAsync(string workFlowTrigger)
    {
        var workflows = await _context.ProjectWorkflows.AsNoTracking().Where(x => x.Trigger == workFlowTrigger).Select(x => new WorkflowContextDto
        {
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
            GroupIds = _context.WorkflowGroups.Where(g => g.WorkflowId == x.Id).Select(g => g.GroupId).ToList(),
        }).Union(_context.ServerWorkflows.AsNoTracking().Where(x => x.Trigger == workFlowTrigger).Select(x => new WorkflowContextDto
        {
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
            GroupIds = _context.WorkflowGroups.Where(g => g.WorkflowId == x.Id).Select(g => g.GroupId).ToList(),
        })).ToListAsync();

        return new ResponseModel<List<WorkflowContextDto>>
        {
            Success = true,
            Message = "Workflows retrieved successfully.",
            Data = workflows
        };
    }
    public async Task<ResponseModel<List<WorkflowContextDto>>> GetServerWorkFlowsByTriggerAsync(string workFlowTrigger, Guid serverId)
    {
        var workflows = await (_context.ServerWorkflows.AsNoTracking().Where(x => x.Trigger == workFlowTrigger && x.ServerId == serverId).Select(x => new WorkflowContextDto
        {
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
            GroupIds = _context.WorkflowGroups.Where(g => g.WorkflowId == x.Id).Select(g => g.GroupId).ToList(),
        })).ToListAsync();

        return new ResponseModel<List<WorkflowContextDto>>
        {
            Success = true,
            Message = "Workflows retrieved successfully.",
            Data = workflows
        };
    }
    public async Task<ResponseModel<List<WorkflowContextDto>>> GetProjectWorkFlowsByTriggerAsync(string workFlowTrigger, Guid projectId)
    {
        var workflows = await _context.ProjectWorkflows.AsNoTracking().Where(x => x.Trigger == workFlowTrigger && x.ProjectId == projectId).Select(x => new WorkflowContextDto
        {
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Trigger = x.Trigger,
            GroupIds = _context.WorkflowGroups.Where(g => g.WorkflowId == x.Id).Select(g => g.GroupId).ToList(),
        }).ToListAsync();

        return new ResponseModel<List<WorkflowContextDto>>
        {
            Success = true,
            Message = "Workflows retrieved successfully.",
            Data = workflows
        };
    }
}
