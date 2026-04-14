using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.GroupScript;

public class RemoveScriptFromGroupCommand : IRequest<ResponseModel<string>>
{   
    public Guid ScriptId { get; set; }
    public Guid GroupId { get; set; }
}

public class RemoveScriptFromGroupCommandHandler : IRequestHandler<RemoveScriptFromGroupCommand, ResponseModel<string>>
{
    private readonly IGroupScriptService _groupScriptService;
    public RemoveScriptFromGroupCommandHandler(IGroupScriptService groupScriptService)
    {
        _groupScriptService = groupScriptService;
    }
    public async Task<ResponseModel<string>> Handle(RemoveScriptFromGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupScriptService.RemoveScriptFromGroupAsync(request.ScriptId, request.GroupId);
    }
}