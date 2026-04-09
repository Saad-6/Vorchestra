using Vorchestra.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Interfaces;

public interface IPlanService
{
    Task<ResponseModel<List<PlanViewDto>>> GetAllPlansAsync(bool? isActive = null, string? query = null);
    Task<ResponseModel<string>> CreatePlanAsync(CreatePlanDto plan);
    Task<ResponseModel<string>> UpdatePlanAsync(UpdatePlanDto plan);
    Task<ResponseModel<string>> DeletePlanAsync(Guid planId);
}
