using Shared.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface IPlanService
{
    Task<ResponseModel<List<PlanViewDto>>> GetAllPlansAsync(FilterModel filter, CancellationToken cancellationToken = default);
    Task<ResponseModel<Guid>> CreatePlanAsync(CreatePlanDto plan);
    Task<ResponseModel<Guid>> UpdatePlanAsync(UpdatePlanDto plan);
    Task<ResponseModel<string>> DeletePlanAsync(Guid planId);
}
