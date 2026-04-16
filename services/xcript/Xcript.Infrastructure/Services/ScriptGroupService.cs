using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.Domain.DataModels;

namespace Xcript.Infrastructure.Services;

public class ScriptGroupService : IScriptGroupService
{
    private readonly XcriptDbContext _context;
    public ScriptGroupService(XcriptDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<string>> AddScriptToGroupAsync(Guid scriptId, Guid groupId, int order, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(groupId, scriptId);

        if (!validationResult.Success)
        {
            return new ResponseModel<string>() { Success = false, Message = validationResult.Message };
        }

        var scriptGroups = validationResult.Data;

        var scriptExistsInGroup = scriptGroups?.Any(m => m.ScriptId == scriptId) ?? false;

        if (scriptExistsInGroup)
        {
            return new ResponseModel<string>() { Success = false, Message = "Script already exists in group" };
        }

        if (order < 1)
            order = 1;
        var orderExists = scriptGroups?.Any(m => m.Order == order) ?? false;
        var maxOrder = scriptGroups.Any() ? (scriptGroups?.Max(m => m.Order) ?? 0) : 0;

        if (orderExists)
        {
            if(order > maxOrder)
            {
                order = maxOrder + 1;
            }
            else
            {
                var scriptGroupsToUpdate = scriptGroups.Where(m => m.Order >= order).ToList();
                foreach (var scriptGrp in scriptGroupsToUpdate)
                {
                    scriptGrp.Order += 1;
                }
            }
        }
        else
        {
            if(order > maxOrder + 1)
            {
                order = maxOrder + 1;
            } 
        }

        var scriptGroup = new ScriptGroup()
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            ScriptId = scriptId,
            Order = order,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.ScriptGroups.Add(scriptGroup);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>() { Success = true, Message = "Script added to group successfully" };
    }
    public async Task<ResponseModel<string>> RemoveScriptFromGroupAsync(Guid scriptId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(groupId, scriptId);
        
        if (!validationResult.Success)
        {
            return new ResponseModel<string>() { Message = validationResult.Message, Success = false };
        }

        var scriptGroups = validationResult.Data;

        var scriptGroup = scriptGroups?.FirstOrDefault(m => m.ScriptId == scriptId);

        if(scriptGroup == null)
        {
            return new ResponseModel<string>() { Success = false, Message = "Script not found in group" };
        }

        _context.ScriptGroups.Remove(scriptGroup);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>() { Success = true, Message = "Script removed from group" };

    }

    private async Task<ResponseModel<List<ScriptGroup>>> ValidateAsync(Guid groupId, Guid scriptId)
    {
        var result = await _context.Groups
        .Where(g => g.Id == groupId)
        .Select(g => new
        {
        ScriptGroups = _context.ScriptGroups
            .Where(sg => sg.GroupId == groupId)
            .ToList(),

        ScriptExists = _context.Scripts
            .Any(s => s.Id == scriptId)
        })
        .FirstOrDefaultAsync();

        var groupExists = result != null;

        var scriptExists = result?.ScriptExists;

        var scriptGroups = result?.ScriptGroups;

        if (!groupExists)
        {
            return new ResponseModel<List<ScriptGroup>>()
            {
                Success = false,
                Message = "Group not found",
                Data = []
            };
        }

        if (!scriptExists.GetValueOrDefault())
        {
            return new ResponseModel<List<ScriptGroup>>()
            {
                Success = false,
                Message = "Script not found",
                Data= []
            };
        }
        return new ResponseModel<List<ScriptGroup>>() { Success = true, Data = scriptGroups };
    }

}
