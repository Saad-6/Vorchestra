using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly VorchestraDbContext _context;
    private readonly IHashService _hashService;

    public TenantService(VorchestraDbContext context, IHashService hashService)
    {
        _context = context;
        _hashService = hashService;
    }

    public async Task<ResponseModel<Guid>> CreateTenantAsync(CreateTenantDto tenant, CancellationToken cancellationToken = default)
    {
        var duplicate = await _context.Tenants.AnyAsync(
            t => t.AdminEmail == tenant.AdminEmail || t.BusinessEmail == tenant.BusinessEmail,
            cancellationToken);

        if (duplicate)
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A tenant with the same admin email or business email already exists.",
                Data = Guid.Empty
            };

        if (string.IsNullOrWhiteSpace(tenant.AdminPassword) || tenant.AdminPassword.Length < 6)
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Admin password is required and must be at least 6 characters long.",
                Data = Guid.Empty
            };

        var newTenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = tenant.Name,
            Description = tenant.Description,
            AdminEmail = tenant.AdminEmail,
            AdminPassword = await _hashService.HashAsync(tenant.AdminPassword),
            BusinessEmail = tenant.BusinessEmail,
            Slug = tenant.Slug,
            Identifier = BuildIdentifier(),
            PhoneNumber = tenant.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _context.Tenants.AddAsync(newTenant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid> { Success = true, Message = "Tenant created successfully.", Data = newTenant.Id };
    }

    public async Task<ResponseModel<Guid>> UpdateTenantAsync(UpdateTenantDto tenant, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenant.Id, cancellationToken);
        if (existing == null)
            return new ResponseModel<Guid> { Success = false, Message = $"Tenant {tenant.Id} not found.", Data = Guid.Empty };

        existing.Name = tenant.Name;
        existing.Description = tenant.Description;
        existing.AdminEmail = tenant.AdminEmail;
        existing.BusinessEmail = tenant.BusinessEmail;
        existing.Slug = tenant.Slug ?? existing.Slug;
        existing.PhoneNumber = tenant.PhoneNumber;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Tenants.Update(existing);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid> { Success = true, Message = "Tenant updated successfully.", Data = existing.Id };
    }

    public async Task<ResponseModel<string>> DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (tenant == null)
            return new ResponseModel<string> { Success = false, Message = "Tenant not found." };

        // Remove all associated data in dependency order
        var tenantProjectIds = await _context.TenantProjects
            .Where(tp => tp.TenantId == tenantId)
            .Select(tp => tp.Id)
            .ToListAsync(cancellationToken);

        if (tenantProjectIds.Count > 0)
        {
            var subscriptions = await _context.TenantSubscriptions
                .Where(s => tenantProjectIds.Contains(s.TenantProjectId))
                .ToListAsync(cancellationToken);
            _context.TenantSubscriptions.RemoveRange(subscriptions);

            var history = await _context.PlanSubscriptionHistory
                .Where(h => h.TenantId == tenantId)
                .ToListAsync(cancellationToken);
            _context.PlanSubscriptionHistory.RemoveRange(history);

            var projects = await _context.TenantProjects
                .Where(tp => tp.TenantId == tenantId)
                .ToListAsync(cancellationToken);
            _context.TenantProjects.RemoveRange(projects);
        }

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string> { Success = true, Message = "Tenant and all associated data deleted successfully.", Data = tenantId.ToString() };
    }

    public async Task<PaginatedResponseModel<TenantViewDto>> GetPaginatedTenantsAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsNoTracking();
        query = filter.ApplyFilters(query);

        var totalCount = await query.CountAsync(cancellationToken);
        var tenants = await query
            .Select(t => new TenantViewDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                PhoneNumber = t.PhoneNumber,
                AdminEmail = t.AdminEmail,
                BusinessEmail = t.BusinessEmail
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponseModel<TenantViewDto>
        {
            Success = true,
            Data = tenants,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    private int BuildIdentifier()
    {
        var max = _context.Tenants.Max(t => (int?)t.Identifier) ?? 0;
        return max + 1;
    }
}
