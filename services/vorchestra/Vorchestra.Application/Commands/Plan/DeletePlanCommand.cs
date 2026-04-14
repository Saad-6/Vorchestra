using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.Application.Commands.Plan;

public class DeletePlanCommand : IRequest<ResponseModel<string>>
{
    public Guid PlanId { get; set; }
}
public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand, ResponseModel<string>>
{
    private readonly IPlanService _planService;
    public DeletePlanCommandHandler(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<ResponseModel<string>> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        return await _planService.DeletePlanAsync(request.PlanId);
    }
}
