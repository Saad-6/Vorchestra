using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Shared.DTO;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class TenantProjectService : ITenantProjectService
{
    private readonly VorchestraDbContext _context;

    public TenantProjectService(VorchestraDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseModel<Guid>> CreateTenantProjectAsync(CreateTenantProjectDto dto, CancellationToken cancellationToken = default)
    {
        var tenantExists = await _context.Tenants.AnyAsync(t => t.Id == dto.TenantId, cancellationToken);
        if (!tenantExists)
            return new ResponseModel<Guid> { Success = false, Message = "Tenant not found.", Data = Guid.Empty };

        var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId, cancellationToken);
        if (!projectExists)
            return new ResponseModel<Guid> { Success = false, Message = "Project not found.", Data = Guid.Empty };

        if (dto.ServerId.HasValue)
        {
            var serverExists = await _context.Servers.AnyAsync(s => s.Id == dto.ServerId.Value, cancellationToken);
            if (!serverExists)
                return new ResponseModel<Guid> { Success = false, Message = "Server not found.", Data = Guid.Empty };
        }

        var duplicate = await _context.TenantProjects
            .AnyAsync(tp => tp.TenantId == dto.TenantId && tp.ProjectId == dto.ProjectId, cancellationToken);
        if (duplicate)
            return new ResponseModel<Guid> { Success = false, Message = "This tenant already has an instance of the specified project.", Data = Guid.Empty };

        if (!string.IsNullOrWhiteSpace(dto.Domain))
        {
            var domainTaken = await _context.TenantProjects
                .AnyAsync(tp => tp.Domain == dto.Domain, cancellationToken);
            if (domainTaken)
                return new ResponseModel<Guid> { Success = false, Message = "The specified domain is already in use.", Data = Guid.Empty };
        }

        var tenantProject = new TenantProject
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            ProjectId = dto.ProjectId,
            ServerId = dto.ServerId,
            Domain = dto.Domain,
            ConnectionString = dto.ConnectionString,
            AssignedPort = dto.AssignedPort,
            Status = TenantProjectStatus.PENDING,
            IsSetupComplete = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _context.TenantProjects.AddAsync(tenantProject, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid> { Success = true, Message = "Tenant project created successfully.", Data = tenantProject.Id };
    }

    public async Task<PaginatedResponseModel<TenantProjectViewDto>> GetTenantProjectsAsync(Guid tenantId, FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.TenantProjects
            .AsNoTracking()
            .Where(tp => tp.TenantId == tenantId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(tp => new TenantProjectViewDto
            {
                Id = tp.Id,
                TenantId = tp.TenantId,
                ProjectId = tp.ProjectId,
                ServerId = tp.ServerId,
                Domain = tp.Domain,
                ConnectionString = tp.ConnectionString,
                AssignedPort = tp.AssignedPort,
                Status = tp.Status,
                IsSetupComplete = tp.IsSetupComplete,
                OnboardedAt = tp.OnboardedAt,
                SuspendedAt = tp.SuspendedAt,
                SuspensionReason = tp.SuspensionReason
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponseModel<TenantProjectViewDto>
        {
            Success = true,
            Message = "Tenant projects retrieved successfully.",
            Data = items,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ResponseModel<TenantProjectViewDto>> GetTenantProjectByIdAsync(Guid tenantProjectId, CancellationToken cancellationToken = default)
    {
        var tp = await _context.TenantProjects
            .AsNoTracking()
            .FirstOrDefaultAsync(tp => tp.Id == tenantProjectId, cancellationToken);

        if (tp == null)
            return new ResponseModel<TenantProjectViewDto> { Success = false, Message = "Tenant project not found." };

        return new ResponseModel<TenantProjectViewDto>
        {
            Success = true,
            Message = "Tenant project retrieved successfully.",
            Data = new TenantProjectViewDto
            {
                Id = tp.Id,
                TenantId = tp.TenantId,
                ProjectId = tp.ProjectId,
                ServerId = tp.ServerId,
                Domain = tp.Domain,
                ConnectionString = tp.ConnectionString,
                AssignedPort = tp.AssignedPort,
                Status = tp.Status,
                IsSetupComplete = tp.IsSetupComplete,
                OnboardedAt = tp.OnboardedAt,
                SuspendedAt = tp.SuspendedAt,
                SuspensionReason = tp.SuspensionReason
            }
        };
    }

    public async Task<ResponseModel<TenantContextDto>> GetTenantContextAsync(Guid tenantProjectId, CancellationToken cancellationToken = default)
    {
        var result = await _context.TenantProjects
            .AsNoTracking()
            .Where(tp => tp.Id == tenantProjectId)
            .Join(_context.Tenants, tp => tp.TenantId, t => t.Id,
                (tp, t) => new TenantContextDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug,
                    Domain = tp.Domain ?? string.Empty,
                    ConnectionString = tp.ConnectionString ?? string.Empty,
                    Port = tp.AssignedPort ?? 0
                })
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
            return new ResponseModel<TenantContextDto> { Success = false, Message = "Tenant project not found." };

        return new ResponseModel<TenantContextDto> { Success = true, Data = result };
    }

    public async Task<ResponseModel<List<TenantProjectViewDto>>> GetRunningTenantProjectsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var items = await _context.TenantProjects
            .AsNoTracking()
            .Where(tp => tp.ProjectId == projectId && tp.Status == TenantProjectStatus.RUNNING)
            .Select(tp => new TenantProjectViewDto
            {
                Id = tp.Id,
                TenantId = tp.TenantId,
                ProjectId = tp.ProjectId,
                ServerId = tp.ServerId,
                Domain = tp.Domain,
                ConnectionString = tp.ConnectionString,
                AssignedPort = tp.AssignedPort,
                Status = tp.Status,
                IsSetupComplete = tp.IsSetupComplete,
                OnboardedAt = tp.OnboardedAt,
                SuspendedAt = tp.SuspendedAt,
                SuspensionReason = tp.SuspensionReason
            })
            .ToListAsync(cancellationToken);

        return new ResponseModel<List<TenantProjectViewDto>> { Success = true, Data = items };
    }

    public async Task<ResponseModel<string>> SuspendTenantProjectAsync(Guid tenantProjectId, string reason, CancellationToken cancellationToken = default)
    {
        var tenantProject = await _context.TenantProjects
            .FirstOrDefaultAsync(tp => tp.Id == tenantProjectId, cancellationToken);

        if (tenantProject == null)
            return new ResponseModel<string> { Success = false, Message = "Tenant project not found." };

        if (tenantProject.Status == TenantProjectStatus.SUSPENDED)
            return new ResponseModel<string> { Success = false, Message = "Tenant project is already suspended." };

        if (tenantProject.Status == TenantProjectStatus.FAILED)
            return new ResponseModel<string> { Success = false, Message = "A failed tenant project cannot be suspended." };

        tenantProject.Status = TenantProjectStatus.SUSPENDED;
        tenantProject.SuspendedAt = DateTimeOffset.UtcNow;
        tenantProject.SuspensionReason = reason;
        tenantProject.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string> { Success = true, Message = "Tenant project suspended successfully." };
    }

    public async Task<ResponseModel<string>> ReactivateTenantProjectAsync(Guid tenantProjectId, CancellationToken cancellationToken = default)
    {
        var tenantProject = await _context.TenantProjects
            .FirstOrDefaultAsync(tp => tp.Id == tenantProjectId, cancellationToken);

        if (tenantProject == null)
            return new ResponseModel<string> { Success = false, Message = "Tenant project not found." };

        if (tenantProject.Status != TenantProjectStatus.SUSPENDED)
            return new ResponseModel<string> { Success = false, Message = "Only suspended tenant projects can be reactivated." };

        tenantProject.Status = TenantProjectStatus.RUNNING;
        tenantProject.SuspendedAt = null;
        tenantProject.SuspensionReason = string.Empty;
        tenantProject.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string> { Success = true, Message = "Tenant project reactivated. A new subscription is required to restore billing." };
    }
}
