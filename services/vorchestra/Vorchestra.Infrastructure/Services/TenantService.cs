using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Shared.DTO;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly VorchestraDbContext _context;

    public TenantService(VorchestraDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<string>> ApplyFreeTrialAsync(Guid tenantId, DateTimeOffset trialEndDate, CancellationToken cancellationToken = default)
    {
        //var existingTenant = await _context.Tenants.FirstOrDefaultAsync(
        //    t => t.Id == tenantId, cancellationToken);

        //if (existingTenant == null)
        //{
        //    return new ResponseModel<string>
        //    {
        //        Success = false,
        //        Message = $"The tenant with Id {tenantId} does not exist."
        //    };
        //}

        //if(existingTenant.Status == TenantStatus.SUBSCRIBED || existingTenant.Status == TenantStatus.FREE_TRIAL)
        //{
        //    return new ResponseModel<string>
        //    {
        //        Success = false,
        //        Message = $"The tenant with Id {tenantId} is already subscribed and cannot be put on a free trial."
        //    };
        //}

        //existingTenant.Status = TenantStatus.FREE_TRIAL;
        //await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Free trial applied successfully."
        };
    }

    public async Task<ResponseModel<string>> ApplyPlanAsync(ApplyPlanDto apply, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _context.Tenants.FirstOrDefaultAsync(
            t => t.Id == apply.TenantId, cancellationToken);

        if (existingTenant == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The tenant with Id {apply.TenantId} does not exist."
            };
        }

        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == apply.PlanId, cancellationToken);

        if(plan == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The plan with Id {apply.PlanId} does not exist."
            };
        }

        existingTenant.IsSetupComplete = true;
        existingTenant.PlanId = apply.PlanId;
        existingTenant.BillingCycle = apply.BillingCycle;

        PlanSubscriptionHistory? subscriptionPlanHistory = null;

        if (existingTenant.Status == TenantStatus.FREE_TRIAL)
        {
            existingTenant.SubscriptionStartDate =  DateTimeOffset.UtcNow;

            existingTenant.SubscriptionEndDate = apply.BillingCycle == BillingCycle.MONTHLY ? existingTenant?.SubscriptionStartDate?.AddMonths(1) : existingTenant?.SubscriptionStartDate?.AddYears(1);

            subscriptionPlanHistory = new PlanSubscriptionHistory
            {
                TenantId = existingTenant.Id,
                IsFreeTrial = true
            };
        }
        else if(existingTenant.Status == TenantStatus.SUBSCRIBED)
        {
            subscriptionPlanHistory = new PlanSubscriptionHistory
            {
                TenantId = existingTenant.Id,
                EndDate = existingTenant.SubscriptionEndDate,
                StartDate = existingTenant.SubscriptionStartDate,
                IsFreeTrial = false
            
            };
            existingTenant.SubscriptionStartDate = existingTenant?.SubscriptionEndDate ?? DateTimeOffset.UtcNow;
           
            existingTenant.SubscriptionEndDate = apply.BillingCycle == BillingCycle.MONTHLY ? existingTenant?.SubscriptionEndDate?.AddMonths(1) : existingTenant?.SubscriptionEndDate?.AddYears(1);
        }
        else
        {
            existingTenant.SubscriptionStartDate = DateTimeOffset.UtcNow;
         
            existingTenant.SubscriptionEndDate = apply.BillingCycle == BillingCycle.MONTHLY ? existingTenant?.SubscriptionStartDate?.AddMonths(1) : existingTenant?.SubscriptionStartDate?.AddYears(1);
        }
        existingTenant.Status = TenantStatus.SUBSCRIBED;
        existingTenant.OnboardedAt = DateTimeOffset.UtcNow;

        _context.Update(existingTenant);

        if(subscriptionPlanHistory != null)
        {
            await _context.AddAsync(subscriptionPlanHistory, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Plan applied successfully."
        };
    }

    public async Task<ResponseModel<string>> CancelSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _context.Tenants.FirstOrDefaultAsync(
            t => t.Id == tenantId, cancellationToken);

        if (existingTenant == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The tenant with Id {tenantId} does not exist."
            };
        }
        if(existingTenant.PlanId == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The tenant with Id {tenantId} does not have an active subscription plan."
            };
        }
        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == existingTenant.PlanId, cancellationToken);

        if (plan == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The plan with Id {existingTenant.PlanId} does not exist."
            };
        }

        var subscriptionHistory = new PlanSubscriptionHistory
        {
            TenantId = existingTenant.Id,
            PlanId = existingTenant.PlanId.Value,
            StartDate = existingTenant.SubscriptionStartDate,
            EndDate = DateTimeOffset.UtcNow,
            IsFreeTrial = existingTenant.Status == TenantStatus.FREE_TRIAL
        };
        existingTenant.PlanId = null;
        existingTenant.SubscriptionStartDate = null;
        existingTenant.SubscriptionEndDate = null;
        existingTenant.Status = TenantStatus.ONBOARDED;

        _context.Update(existingTenant);
        await _context.AddAsync(subscriptionHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Subscription cancelled successfully."
        };
    }

    public async Task<ResponseModel<Guid>> CreateTenantAsync(CreateTenantDto tenant, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _context.Tenants.AnyAsync(
            t => t.AdminEmail == tenant.AdminEmail 
            || t.BusinessEmail == tenant.BusinessEmail 
            || t.Domain == tenant.Domain, cancellationToken);

        if(existingTenant)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A tenant with the same admin email, business email, or domain already exists.",
                Data = Guid.Empty
            };
        }
        var newTenant = new Tenant();

        newTenant.Name = tenant.Name;
        newTenant.Description = tenant.Description;
        newTenant.AdminEmail = tenant.AdminEmail;
        newTenant.BusinessEmail = tenant.BusinessEmail;
        newTenant.Domain = tenant.Domain;
        newTenant.Slug = tenant.Slug;
        newTenant.Identifier = tenant.Domain + tenant.Slug;
        newTenant.PhoneNumber = tenant.PhoneNumber;

        await _context.AddAsync(newTenant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Tenant created successfully.",
            Data = newTenant.Id
        };
    }

    public async Task<PaginatedResponseModel<TenantViewDto>> GetPaginatedTenantsAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsQueryable();

        query = filter.ApplyFilters(query);

        var totalCount = await query.CountAsync(cancellationToken);
        var tenants = await query.ToListAsync(cancellationToken);

        var tenantDto = tenants.Select(t => new TenantViewDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            PhoneNumber = t.PhoneNumber,
            AdminEmail = t.AdminEmail,
            BusinessEmail = t.BusinessEmail,
            Domain = t.Domain,
            Status = t.Status,
            SuspendedAt = t.SuspendedAt,
            SuspensionReason = t.SuspensionReason,
            SubscriptionStartDate = t.SubscriptionStartDate,
            SubscriptionEndDate = t.SubscriptionEndDate,
            BillingCycle = t.BillingCycle,
            Slug = t.Slug,
            Identifier = t.Identifier,
            IsSetupComplete = t.IsSetupComplete,
            OnboardedAt = t.OnboardedAt,
            PlanId = t.PlanId,
            ServerId = t.ServerId
        }).ToList();

        return new PaginatedResponseModel<TenantViewDto>
        {
            Success = true,
            Data = tenantDto,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ResponseModel<TenantContextDto>> GetTenantContextByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
        if (tenant == null)
        {
            return new ResponseModel<TenantContextDto> { Success = false };
        }
        return new ResponseModel<TenantContextDto>
        {
            Success = true,
            Data = new TenantContextDto
            {
                Id = tenantId,
                Name = tenant.Name,
                Slug = tenant.Slug,
                Domain = tenant.Domain,
                ConnectionString = BuildConnectionString(tenant),
                Port = GetAvailablePort()
            }
        };

    }

    private string BuildConnectionString(Tenant tenant)
    {
        return $"Server={tenant.Domain};Database={tenant.Identifier};User Id=your_user;Password=your_password;";
    }
    private int GetAvailablePort()
    {
        // Implement logic to get an available port number
        // This is a placeholder implementation and should be replaced with actual logic
        return new Random().Next(10000, 60000);
    }
    public async Task<ResponseModel<string>> ReactivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _context.Tenants.FirstOrDefaultAsync(
            t => t.Id == tenantId, cancellationToken);

        if (existingTenant == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The tenant with Id {tenantId} does not exist."
            };
        }
        existingTenant.Status = TenantStatus.PENDING;
        existingTenant.SuspendedAt = null;
        existingTenant.SuspensionReason = "Previously suspended for : " + existingTenant.SuspensionReason;

        _context.Update(existingTenant);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Tenant reactivated successfully, please proceed with a new subscription."
        };
    }

    public async Task<ResponseModel<string>> SuspendTenantAsync(Guid tenantId, string reason, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _context.Tenants.FirstOrDefaultAsync(
            t => t.Id == tenantId, cancellationToken);

        if (existingTenant == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"The tenant with Id {tenantId} does not exist."
            };
        }

        existingTenant.Status = TenantStatus.SUSPENDED;
        existingTenant.SuspendedAt = DateTimeOffset.UtcNow;
        existingTenant.SuspensionReason = reason;
        _context.Update(existingTenant);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Tenant suspended successfully."
        };
    }

    public async Task<ResponseModel<Guid>> UpdateTenantAsync(UpdateTenantDto tenant, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _context.Tenants.FirstOrDefaultAsync(
            t => t.Id == tenant.Id, cancellationToken);

        if (existingTenant == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = $"The tenant with Id {tenant.Id} does not exist.",
                Data = Guid.Empty
            };
        }

        existingTenant.Name = tenant.Name;
        existingTenant.Description = tenant.Description;
        existingTenant.AdminEmail = tenant.AdminEmail;
        existingTenant.BusinessEmail = tenant.BusinessEmail;
        existingTenant.Domain = tenant.Domain;
        existingTenant.Slug = tenant?.Slug ?? existingTenant.Slug;
        existingTenant.Status = tenant?.Status ?? existingTenant.Status;

        if(tenant?.SuspendedAt != null)
        {
            existingTenant.SuspendedAt = tenant.SuspendedAt.Value;
        }
        if(tenant?.SuspensionReason != null)
        {
            existingTenant.SuspensionReason = tenant.SuspensionReason;
        }

        _context.Tenants.Update(existingTenant);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Tenant updated successfully.",
            Data = existingTenant.Id
        };
    }
}