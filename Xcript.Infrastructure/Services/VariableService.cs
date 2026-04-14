using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.Application.Queries.Variable;
using Xcript.Domain.DataModels;
using Xcript.DTOs;

namespace Xcript.Infrastructure.Services;

public class VariableService : IVariableService
{
    private readonly XcriptDbContext _context;

    public VariableService(XcriptDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<Guid>> CreateAsync(CreateVariableDto dto, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.Variables.AnyAsync(v => v.Name == dto.Name, cancellationToken);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A variable with the same name already exists.",
                Data = Guid.Empty
            };
        }
        var variable = new Variable
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Source = dto.Source,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        await _context.AddAsync(variable, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Variable created successfully.",
            Data = variable.Id
        };
    }

    public async Task<ResponseModel<string>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var variable = await _context.Variables.FindAsync(new object[] { id }, cancellationToken);
        if(variable == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Variable not found.",
                Data = null
            };
        }
        var scriptVariables = await _context.ScriptVariables.Where(sv => sv.VariableId == id).ToListAsync(cancellationToken);

        _context.Variables.Remove(variable);
        
        _context.ScriptVariables.RemoveRange(scriptVariables);

        await _context.SaveChangesAsync(cancellationToken);


        return new ResponseModel<string>
        {
            Success = true,
            Message = "Variable deleted successfully.",
            Data = id.ToString()
        };
    }

    public async Task<ResponseModel<VariableViewDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var variable = await _context.Variables.Select(m => new VariableViewDto
        {
            Id = m.Id,
            Name = m.Name,
            Source = m.Source,
            Description = m.Description,
        }).FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if( variable == null)
        {
            return new ResponseModel<VariableViewDto>
            {
                Success = false,
                Message = "Variable not found.",
                Data = null
            };
        }
        return new ResponseModel<VariableViewDto>
        {
            Success = true,
            Message = "Variable retrieved successfully.",
            Data = variable
        };
    }

    public async Task<PaginatedResponseModel<VariableViewDto>> GetPaginatedVariablesAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Variables.AsNoTracking();
        
        query = filter.ApplyFilters(query);
        
        var count = await query.CountAsync(cancellationToken);

        var variables = await query.Select(m => new VariableViewDto
        {
            Id = m.Id,
            Name = m.Name,
            Source = m.Source,
            Description = m.Description,
        }).ToListAsync(cancellationToken);

        return new PaginatedResponseModel<VariableViewDto>
        {
            Success = true,
            Message = "Variables retrieved successfully.",
            Data = variables,
            TotalCount = count,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ResponseModel<Guid>> UpdateAsync(Guid id, UpdateVariableDto dto, CancellationToken cancellationToken = default)
    {
        var variable = await _context.Variables.FindAsync(new object[] { id }, cancellationToken);
        if (variable == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Variable not found.",
                Data = Guid.Empty
            };
        }
        var nameCheck = await _context.Variables.AnyAsync(v => v.Name == dto.Name && v.Id != id, cancellationToken);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A variable with the same name already exists.",
                Data = Guid.Empty
            };
        }

        variable.Name = dto.Name;
        variable.Source = dto.Source;
        variable.Description = dto.Description;

        _context.Variables.Update(variable);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Variable updated successfully.",
            Data = variable.Id
        };
    }
}
