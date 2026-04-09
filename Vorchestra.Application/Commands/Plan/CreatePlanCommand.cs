using MediatR;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Models;
using Vorchestra.DTOs;

namespace Vorchestra.Application.Commands.Plan;

public class CreatePlanCommand: CreatePlanDto, IRequest<ResponseModel<string>>
{
}
public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, ResponseModel<string>>
{
    private readonly IPlanService _planService;
    public CreatePlanCommandHandler(IPlanService planService)
    {
        _planService = planService;
    }
    public async Task<ResponseModel<string>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        return await _planService.CreatePlanAsync(request);
    }
}
