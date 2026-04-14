using MediatR;
using Shared.Application.Models;
using Vorchestra.Application.Interfaces;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Plan;

public class CreatePlanCommand: CreatePlanDto, IRequest<ResponseModel<Guid>>
{
}
public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, ResponseModel<Guid>>
{
    private readonly IPlanService _planService;
    public CreatePlanCommandHandler(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        return await _planService.CreatePlanAsync(request);
    }
}
