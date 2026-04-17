using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class PlanService : IPlanService
{
    private readonly VorchestraDbContext _context;
    public PlanService(VorchestraDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<Guid>> CreatePlanAsync(CreatePlanDto plan)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == plan.ProjectId);
        if (!projectExists)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "The specified project does not exist.",
                Data = Guid.Empty
            };
        }

        var existingPlan = await _context.Plans.AnyAsync(p => p.Name == plan.Name || p.Slug == plan.Slug);
        if (existingPlan)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A plan with the same name or slug already exists.",
                Data = Guid.Empty
            };
        }

        var newPlan = new Plan
        {
            Id = Guid.NewGuid(),
            ProjectId = plan.ProjectId,
            Name = plan.Name,
            Slug = plan.Slug,
            MonthlyPrice = plan.MonthlyPrice,
            AnnualPrice = plan.AnnualPrice,
            MaxRam = plan.MaxRam,
            MaxStorageGb = plan.MaxStorageGb,
            MaxProducts = plan.MaxProducts,
            IsActive = plan.IsActive
        };

        await _context.Plans.AddAsync(newPlan);
        await _context.SaveChangesAsync();

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Plan created successfully.",
            Data = newPlan.Id
        };
    }

    public async Task<ResponseModel<string>> DeletePlanAsync(Guid planId)
    {
        var existingPlan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == planId);
        if (existingPlan == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "A plan with the specified ID does not exist.",
                Data = null
            };
        }
        _context.Plans.Remove(existingPlan);
        await _context.SaveChangesAsync();

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Plan deleted successfully.",
            Data = planId.ToString()
        };
    }

    public async Task<ResponseModel<List<PlanViewDto>>> GetAllPlansAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var plansQuery = _context.Plans.AsQueryable();

        plansQuery = filter.ApplyFilters(plansQuery);

        var plans = await plansQuery.ToListAsync(cancellationToken);

        var planDtos = plans.Select(p => new PlanViewDto
        {
            Id = p.Id,
            ProjectId = p.ProjectId,
            Name = p.Name,
            Slug = p.Slug,
            MonthlyPrice = p.MonthlyPrice,
            AnnualPrice = p.AnnualPrice,
            MaxRam = p.MaxRam,
            MaxStorageGb = p.MaxStorageGb,
            MaxProducts = p.MaxProducts,
            IsActive = p.IsActive
        }).ToList();

        return new ResponseModel<List<PlanViewDto>>
        {
            Success = true,
            Message = "Plans retrieved successfully.",
            Data = planDtos
        };
    }

    public async Task<ResponseModel<Guid>> UpdatePlanAsync(UpdatePlanDto plan)
    {
        var existingPlan = await _context.Plans.FirstOrDefaultAsync(p => p.Id == plan.Id);
        if (existingPlan == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A plan with the specified ID does not exist.",
                Data = Guid.Empty
            };
        }

        var projectExists = await _context.Projects.AnyAsync(p => p.Id == plan.ProjectId);
        if (!projectExists)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "The specified project does not exist.",
                Data = Guid.Empty
            };
        }

        var nameCheck = await _context.Plans.AnyAsync(p => (p.Name == plan.Name || p.Slug == plan.Slug) && p.Id != plan.Id);
        if(nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Another plan with the same name or slug already exists.",
                Data = Guid.Empty
            };
        }

        existingPlan.ProjectId = plan.ProjectId;
        existingPlan.Name = plan.Name;
        existingPlan.Slug = plan.Slug;
        existingPlan.MonthlyPrice = plan.MonthlyPrice;
        existingPlan.AnnualPrice = plan.AnnualPrice;
        existingPlan.MaxRam = plan.MaxRam;
        existingPlan.MaxStorageGb = plan.MaxStorageGb;
        existingPlan.MaxProducts = plan.MaxProducts;
        existingPlan.IsActive = plan.IsActive;

        _context.Plans.Update(existingPlan);
        await _context.SaveChangesAsync();

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Plan updated successfully.",
            Data = existingPlan.Id
        };
    }
}