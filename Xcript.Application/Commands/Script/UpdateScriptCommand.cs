using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Commands.Script;

public class UpdateScriptCommand : UpdateScriptDto, IRequest<ResponseModel<Guid>>
{
}
public class UpdateScriptCommandHandler : IRequestHandler<UpdateScriptCommand, ResponseModel<Guid>>
{
    private readonly IScriptService _scriptService;
    public UpdateScriptCommandHandler(IScriptService scriptService)
    {
        _scriptService = scriptService;
    }
    public async Task<ResponseModel<Guid>> Handle(UpdateScriptCommand request, CancellationToken cancellationToken)
    {
        return await _scriptService.UpdateScriptAsync(request, cancellationToken);
    }
}
