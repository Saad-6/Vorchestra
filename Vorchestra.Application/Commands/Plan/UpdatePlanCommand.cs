using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Plan;

public class UpdatePlanCommand : UpdatePlanDto, IRequest<ResponseModel<Guid>>
{
}
public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, ResponseModel<Guid>>
{
    private readonly IPlanService _planService;
    public UpdatePlanCommandHandler(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<ResponseModel<Guid>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        return await _planService.UpdatePlanAsync(request);
    }
}
