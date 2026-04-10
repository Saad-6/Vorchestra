using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Queries.Plan;

public class GetAllPlansQuery : IRequest<ResponseModel<List<PlanViewDto>>>
{
    public bool? IsActive { get; set; }
    public string? Query { get; set; }
}

public class GetAllPlansQueryHandler : IRequestHandler<GetAllPlansQuery, ResponseModel<List<PlanViewDto>>>
{
    private readonly IPlanService _planService;
    public GetAllPlansQueryHandler(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<ResponseModel<List<PlanViewDto>>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        return await _planService.GetAllPlansAsync(request.IsActive, request.Query);
    }
}