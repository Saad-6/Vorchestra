using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.GroupScript;

public class AddScriptToGroupCommand : IRequest<ResponseModel<string>>
{
    public Guid ScriptId { get; set; }
    public Guid GroupId { get; set; }
    public int Order { get; set; }
}
public class AddScriptToGroupCommandHandler : IRequestHandler<AddScriptToGroupCommand, ResponseModel<string>>
{
    private readonly IGroupScriptService _groupScriptService;
    public AddScriptToGroupCommandHandler(IGroupScriptService groupScriptService)
    {
        _groupScriptService = groupScriptService;
    }
    public async Task<ResponseModel<string>> Handle(AddScriptToGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupScriptService.AddScriptToGroupAsync(request.ScriptId, request.GroupId, request.Order);
    }
}
