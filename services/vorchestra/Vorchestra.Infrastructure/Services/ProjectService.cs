using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly VorchestraDbContext _context;
    public ProjectService(VorchestraDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseModel<Guid>> CreateProjectAsync(CreateProjectDto project)
    {
        var nameCheck = await _context.Projects.AnyAsync(p => p.Name == project.Name);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A project with the same name already exists.",
                Data = Guid.Empty
            };
        }

        var newProject = new Project
        {
            Id = Guid.NewGuid(),
            Name = project.Name,
            Description = project.Description,
            Url = project.Url,
            IsRelative = project.IsRelative,
            Branch = project.Branch,
            PersonalAccessToken = project.PersonalAccessToken,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _context.Projects.AddAsync(newProject);
        await _context.SaveChangesAsync();

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Project created successfully.",
            Data = newProject.Id
        };
    }

    public async Task<ResponseModel<string>> DeleteProjectAsync(Guid projectId)
    {
        var existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
        if (existingProject == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "A project with the specified ID does not exist.",
                Data = null
            };
        }

        _context.Projects.Remove(existingProject);
        await _context.SaveChangesAsync();

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Project deleted successfully.",
            Data = projectId.ToString()
        };
    }

    public async Task<PaginatedResponseModel<ProjectViewDto>> GetAllProjectsAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsQueryable();

        query = filter.ApplyFilters(query);

        var totalCount = await query.CountAsync(cancellationToken);

        var projects = await query
            .Select(p => new ProjectViewDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Url = p.Url,
                IsRelative = p.IsRelative,
                Branch = p.Branch
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponseModel<ProjectViewDto>
        {
            Success = true,
            Message = "Projects retrieved successfully.",
            Data = projects,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ResponseModel<Guid>> UpdateProjectAsync(UpdateProjectDto project)
    {
        var existingProject = await _context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        if (existingProject == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A project with the specified ID does not exist.",
                Data = Guid.Empty
            };
        }

        var nameCheck = await _context.Projects.AnyAsync(p => p.Name == project.Name && p.Id != project.Id);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Another project with the same name already exists.",
                Data = Guid.Empty
            };
        }

        existingProject.Name = project.Name;
        existingProject.Description = project.Description;
        existingProject.Url = project.Url;
        existingProject.IsRelative = project.IsRelative;
        existingProject.Branch = project.Branch;
        existingProject.PersonalAccessToken = project.PersonalAccessToken;
        existingProject.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Projects.Update(existingProject);
        await _context.SaveChangesAsync();

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Project updated successfully.",
            Data = existingProject.Id
        };
    }
}
