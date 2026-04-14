using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.Domain.DataModels;
using Xcript.DTOs;

namespace Xcript.Infrastructure.Services;

public class GroupService : IGroupService
{
    private readonly XcriptDbContext _context;
    public GroupService(XcriptDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<Guid>> CreateGroupAsync(CreateGroupDto group, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.Groups.AnyAsync(g => g.Name == group.Name, cancellationToken);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Group name already exists.",
                Data = Guid.Empty
            };
        }
        var newGroup = new Group
        {
            Id = Guid.NewGuid(),
            Name = group.Name,
            Description = group.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Groups.AddAsync(newGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Group created successfully.",
            Data = newGroup.Id
        };
    }

    public async Task<ResponseModel<string>> DeleteGroupAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await _context.Groups.FindAsync(new object[] { groupId }, cancellationToken);
        if (group == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Group not found.",
                Data = null
            };
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Group deleted successfully.",
            Data = groupId.ToString()
        };
    }

    public async Task<ResponseModel<GroupViewDto>> GetGroupByIdAsync(
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        var group = await _context.Groups
            .AsNoTracking()
            .Where(g => g.Id == groupId)
            .Select(g => new GroupViewDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                Scripts = (
                    from sg in _context.ScriptGroups
                    join s in _context.Scripts on sg.ScriptId equals s.Id
                    where sg.GroupId == g.Id
                    select new ScriptViewDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Content = s.Content,
                        Order = sg.Order
                    }
                ).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return group is not null
            ? new ResponseModel<GroupViewDto>
            {
                Success = true,
                Message = "Group retrieved successfully.",
                Data = group
            }
            : new ResponseModel<GroupViewDto>
            {
                Success = false,
                Message = "Group not found.",
                Data = null
            };
    }

    public async Task<PaginatedResponseModel<GroupViewDto>> GetPaginatedGroupsAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Groups.AsNoTracking();

        query = filter.ApplyFilters(query);

        var totalCount = await query.CountAsync(cancellationToken);

        var groups = await query
            .Select(g => new GroupViewDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponseModel<GroupViewDto>
        {
            Success = true,
            Message = "Groups retrieved successfully.",
            Data = groups,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ResponseModel<Guid>> UpdateGroupAsync(UpdateGroupDto group, CancellationToken cancellationToken = default)
    {
        var existingGroup = await _context.Groups.FindAsync(new object[] { group.Id }, cancellationToken);
        if(existingGroup == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Group not found.",
                Data = Guid.Empty
            };
        }
        var nameCheck = await _context.Groups.AnyAsync(g => g.Name == group.Name && g.Id != group.Id, cancellationToken);
        if(nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Group name already exists.",
                Data = Guid.Empty
            };
        }
        existingGroup.Name = group.Name;
        existingGroup.Description = group.Description;
        
        _context.Groups.Update(existingGroup);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Group updated successfully.",
            Data = existingGroup.Id
        };
    }
}
