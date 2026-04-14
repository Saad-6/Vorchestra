using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.Script;

public class DeleteScriptCommand : IRequest<ResponseModel<string>>
{
    public Guid Id { get; set; }
}

public class DeleteScriptCommandHandler : IRequestHandler<DeleteScriptCommand, ResponseModel<string>>
{
    private readonly IScriptService _scriptService;
    public DeleteScriptCommandHandler(IScriptService scriptService)
    {
        _scriptService = scriptService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteScriptCommand request, CancellationToken cancellationToken)
    {
        return await _scriptService.DeleteScriptAsync(request.Id, cancellationToken);
    }
}
