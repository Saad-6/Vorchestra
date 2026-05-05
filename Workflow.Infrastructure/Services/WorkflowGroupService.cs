using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Workflow.Application.Interfaces;
using Workflow.Domain.DataModels;

namespace Workflow.Infrastructure.Services;

public class WorkflowGroupService : IWorkflowGroupService
{
    private readonly WorkflowDbContext _dbContext;

    public WorkflowGroupService(WorkflowDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ResponseModel<string>> AddGroupToWorkflowAsync(Guid workflowId, Guid groupId, string groupName, CancellationToken cancellationToken = default)
    {
        var workflowGroup = await _dbContext.WorkflowGroups.Where(wg => wg.WorkflowId == workflowId && wg.GroupId == groupId).FirstOrDefaultAsync(cancellationToken);
        if (workflowGroup != null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Workflow group already exists."
            };
        }

        var maxOrder = await _dbContext.WorkflowGroups.Where(wg => wg.WorkflowId == workflowId).MaxAsync(wg => (int?)wg.Order, cancellationToken) ?? 0;

        var newWorkflowGroup = new WorkflowGroup
        {
            Id = Guid.NewGuid(),
            WorkflowId = workflowId,
            GroupId = groupId,
            GroupName = groupName,
            Order = maxOrder + 1
        };
        
        _dbContext.WorkflowGroups.Add(newWorkflowGroup);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Group added to workflow successfully."
        };
    }

    public async Task<ResponseModel<string>> ChangeGroupOrderInWorkflowAsync(
        Guid workflowId,
        Guid groupId,
        int newOrder,
        CancellationToken cancellationToken = default)
    {
        var workflowGroups = await _dbContext.WorkflowGroups
            .Where(wg => wg.WorkflowId == workflowId)
            .ToListAsync(cancellationToken);

        if (!workflowGroups.Any())
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "No groups found for this workflow."
            };
        }

        var target = workflowGroups.FirstOrDefault(wg => wg.GroupId == groupId);

        if (target == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Workflow group does not exist."
            };
        }

        var maxOrder = workflowGroups.Max(wg => wg.Order);

        // Clamp newOrder within valid range
        newOrder = Math.Max(1, Math.Min(newOrder, maxOrder));

        var oldOrder = target.Order;

        if (oldOrder == newOrder)
        {
            return new ResponseModel<string>
            {
                Success = true,
                Message = "Order unchanged."
            };
        }

        if (newOrder < oldOrder)
        {
            // Moving UP
            foreach (var item in workflowGroups
                .Where(wg => wg.Order >= newOrder && wg.Order < oldOrder))
            {
                item.Order += 1;
            }
        }
        else
        {
            // Moving DOWN
            foreach (var item in workflowGroups
                .Where(wg => wg.Order > oldOrder && wg.Order <= newOrder))
            {
                item.Order -= 1;
            }
        }

        target.Order = newOrder;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Group order updated successfully."
        };
    }

    public async Task<ResponseModel<string>> RemoveGroupFromWorkflowAsync(Guid workflowId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var workflowGroup = await _dbContext.WorkflowGroups.Where(wg => wg.Id == workflowId && wg.GroupId == groupId).FirstOrDefaultAsync(cancellationToken);
        if (workflowGroup == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Workflow group does not exist."
            };
        }
        _dbContext.WorkflowGroups.Remove(workflowGroup);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Group removed from workflow successfully."
        };
    }
}
