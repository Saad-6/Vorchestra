using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Plan;

public class UpdatePlanCommand : UpdatePlanDto, IRequest<ResponseModel<string>>
{
}
public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, ResponseModel<string>>
{
    private readonly IPlanService _planService;
    public UpdatePlanCommandHandler(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<ResponseModel<string>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        return await _planService.UpdatePlanAsync(request);
    }
}
