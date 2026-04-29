using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.Constants;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class TenantSubscriptionService : ITenantSubscriptionService
{
    private readonly VorchestraDbContext _context;

    public TenantSubscriptionService(VorchestraDbContext context)
    {
        _context = context;
    }

    public async Task<ResponseModel<string>> ApplyPlanAsync(ApplyPlanDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validate TenantProject exists
        var tenantProject = await _context.TenantProjects
            .FirstOrDefaultAsync(tp => tp.Id == dto.TenantProjectId, cancellationToken);

        if (tenantProject == null)
            return Fail("Tenant project not found.");

        // 2. Block subscriptions on non-subscribable instances
        if (tenantProject.Status == TenantProjectStatus.SUSPENDED)
            return Fail("Cannot apply a plan to a suspended tenant project. Reactivate it first.");

        if (tenantProject.Status == TenantProjectStatus.FAILED)
            return Fail("Cannot apply a plan to a failed tenant project.");

        // 3. Validate plan exists and is active
        var plan = await _context.Plans
            .FirstOrDefaultAsync(p => p.Id == dto.PlanId, cancellationToken);

        if (plan == null)
            return Fail("Plan not found.");

        if (!plan.IsActive)
            return Fail("The specified plan is not currently active.");

        // 4. Check for an existing active subscription on this project instance
        var existingSubscription = await _context.TenantSubscriptions
            .FirstOrDefaultAsync(s => s.TenantProjectId == dto.TenantProjectId
                                   && s.Status == SubscriptionStatus.ACTIVE, cancellationToken);

        if (existingSubscription != null)
        {
            // Same plan — no-op
            if (existingSubscription.PlanId == dto.PlanId)
                return Fail("This tenant project is already subscribed to the specified plan.");

            // Different plan — cancel the current one before creating the new one
            existingSubscription.Status = SubscriptionStatus.CANCELLED;
            existingSubscription.EndDate = DateTimeOffset.UtcNow;
            existingSubscription.UpdatedAt = DateTimeOffset.UtcNow;
        }

        // 5. Calculate subscription period from billing cycle
        var startDate = DateTimeOffset.UtcNow;
        var endDate = dto.BillingCycle == BillingCycle.ANNUALLY
            ? startDate.AddYears(1)
            : startDate.AddMonths(1);

        // 6. Create the new subscription
        var newSubscription = new TenantSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenantProject.TenantId,
            TenantProjectId = dto.TenantProjectId,
            PlanId = dto.PlanId,
            BillingCycle = dto.BillingCycle,
            IsFreeTrial = false,
            StartDate = startDate,
            EndDate = endDate,
            Status = SubscriptionStatus.ACTIVE,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _context.TenantSubscriptions.AddAsync(newSubscription, cancellationToken);

        // 7. Write history entry
        await _context.PlanSubscriptionHistory.AddAsync(new PlanSubscriptionHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantProject.TenantId,
            TenantProjectId = dto.TenantProjectId,
            PlanId = dto.PlanId,
            IsFreeTrial = false,
            StartDate = startDate,
            EndDate = endDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string> { Success = true, Message = "Plan applied successfully." };
    }

    public async Task<ResponseModel<string>> ApplyFreeTrialAsync(Guid tenantProjectId, DateTimeOffset trialEndDate, CancellationToken cancellationToken = default)
    {
        // 1. Validate TenantProject exists
        var tenantProject = await _context.TenantProjects
            .FirstOrDefaultAsync(tp => tp.Id == tenantProjectId, cancellationToken);

        if (tenantProject == null)
            return Fail("Tenant project not found.");

        // 2. Block on non-subscribable instances
        if (tenantProject.Status == TenantProjectStatus.SUSPENDED)
            return Fail("Cannot apply a free trial to a suspended tenant project.");

        if (tenantProject.Status == TenantProjectStatus.FAILED)
            return Fail("Cannot apply a free trial to a failed tenant project.");

        // 3. Trial end date must be in the future
        if (trialEndDate <= DateTimeOffset.UtcNow)
            return Fail("Trial end date must be in the future.");

        // 4. Free trial is once per project instance — check all history, regardless of status
        var trialAlreadyUsed = await _context.TenantSubscriptions
            .AnyAsync(s => s.TenantProjectId == tenantProjectId && s.IsFreeTrial, cancellationToken);

        if (trialAlreadyUsed)
            return Fail("A free trial has already been used for this project instance.");

        // 5. Cannot start a trial while an active subscription exists
        var activeSubscription = await _context.TenantSubscriptions
            .AnyAsync(s => s.TenantProjectId == tenantProjectId
                        && s.Status == SubscriptionStatus.ACTIVE, cancellationToken);

        if (activeSubscription)
            return Fail("An active subscription already exists. Cancel it before starting a free trial.");

        var startDate = DateTimeOffset.UtcNow;

        // 6. Create trial subscription
        var trial = new TenantSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenantProject.TenantId,
            TenantProjectId = tenantProjectId,
            PlanId = Guid.Empty, // No paid plan during trial
            BillingCycle = "FREE_TRIAL",
            IsFreeTrial = true,
            StartDate = startDate,
            EndDate = trialEndDate,
            Status = SubscriptionStatus.ACTIVE,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await _context.TenantSubscriptions.AddAsync(trial, cancellationToken);

        // 7. Write history
        await _context.PlanSubscriptionHistory.AddAsync(new PlanSubscriptionHistory
        {
            Id = Guid.NewGuid(),
            TenantId = tenantProject.TenantId,
            TenantProjectId = tenantProjectId,
            PlanId = Guid.Empty,
            IsFreeTrial = true,
            StartDate = startDate,
            EndDate = trialEndDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string> { Success = true, Message = "Free trial started successfully." };
    }

    public async Task<ResponseModel<string>> CancelSubscriptionAsync(Guid tenantProjectId, CancellationToken cancellationToken = default)
    {
        // 1. Find the active subscription for this project instance
        var subscription = await _context.TenantSubscriptions
            .FirstOrDefaultAsync(s => s.TenantProjectId == tenantProjectId
                                   && s.Status == SubscriptionStatus.ACTIVE, cancellationToken);

        if (subscription == null)
            return Fail("No active subscription found for this tenant project.");

        // 2. Cancel — access continues until EndDate (no immediate cut-off)
        subscription.Status = SubscriptionStatus.CANCELLED;
        subscription.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var accessUntil = subscription.EndDate.HasValue
            ? $" Access continues until {subscription.EndDate.Value:yyyy-MM-dd}."
            : string.Empty;

        return new ResponseModel<string>
        {
            Success = true,
            Message = $"Subscription cancelled successfully.{accessUntil}"
        };
    }

    private static ResponseModel<string> Fail(string message) =>
        new ResponseModel<string> { Success = false, Message = message };
}
