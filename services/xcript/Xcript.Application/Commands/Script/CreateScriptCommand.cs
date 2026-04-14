using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Commands.Script;

public class CreateScriptCommand : CreateScriptDto, IRequest<ResponseModel<Guid>>
{
}

public class CreateScriptCommandHandler : IRequestHandler<CreateScriptCommand, ResponseModel<Guid>>
{
    private readonly IScriptService _scriptService;
    public CreateScriptCommandHandler(IScriptService scriptService)
    {
        _scriptService = scriptService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateScriptCommand request, CancellationToken cancellationToken)
    {
        return await _scriptService.CreateScriptAsync(request, cancellationToken);
    }
}