using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.Domain.DataModels;
using Xcript.DTOs;

namespace Xcript.Infrastructure.Services;

public class ScriptService : IScriptService
{
    private readonly XcriptDbContext _context;
    public ScriptService(XcriptDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<Guid>> CreateScriptAsync(CreateScriptDto script, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.Scripts.AnyAsync(s => s.Name == script.Name, cancellationToken);

        if(nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Script name already exists.",
                Data = Guid.Empty
            };
        }

        var newScript = new Script
        {
            Id = Guid.NewGuid(),
            Name = script.Name,
            Description = script.Description,
            Content = script.Content,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _context.AddAsync(newScript, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Script created successfully.",
            Data = newScript.Id
        };
    }

    public async Task<ResponseModel<string>> DeleteScriptAsync(Guid scriptId, CancellationToken cancellationToken = default)
    {
        var script = await _context.Scripts.FindAsync(new object[] { scriptId }, cancellationToken);
        if (script == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Script not found.",
                Data = null
            };
        }

        _context.Scripts.Remove(script);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Script deleted successfully.",
            Data = scriptId.ToString()
        };
    }

    public async Task<PaginatedResponseModel<ScriptViewDto>> GetPaginatedScriptsAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Scripts.AsNoTracking();
        
        query = filter.ApplyFilters(query);

        var totalCount = await query.CountAsync(cancellationToken);

        var scripts = await query
            .Select(s => new ScriptViewDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Content = s.Content,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponseModel<ScriptViewDto>
        {
            Data = scripts,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ResponseModel<ScriptViewDto>> GetScriptByIdAsync(Guid scriptId, CancellationToken cancellationToken = default)
    {
        var script = await _context.Scripts.FindAsync(new object[] { scriptId }, cancellationToken);
        
        if(script == null)
        {
            return new ResponseModel<ScriptViewDto>
            {
                Success = false,
                Message = "Script not found.",
                Data = null
            };
        }
        
        return new ResponseModel<ScriptViewDto>
        {
            Success = true,
            Message = "Script retrieved successfully.",
            Data = new ScriptViewDto
            {
                Id = script.Id,
                Name = script.Name,
                Description = script.Description,
                Content = script.Content
            }
        };
    }

    public async Task<ResponseModel<Guid>> UpdateScriptAsync(UpdateScriptDto script, CancellationToken cancellationToken = default)
    {
        var existingScript = await _context.Scripts.FindAsync(new object[] { script.Id }, cancellationToken);
        
        if (existingScript == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Script not found.",
                Data = Guid.Empty
            };
        }

        existingScript.Name = script.Name;
        existingScript.Description = script.Description;
        existingScript.Content = script.Content;
        existingScript.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Scripts.Update(existingScript);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Script updated successfully.",
            Data = existingScript.Id
        };
    }
}
