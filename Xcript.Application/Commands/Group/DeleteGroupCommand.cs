using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;

namespace Xcript.Application.Commands.Group;

public class DeleteGroupCommand : IRequest<ResponseModel<string>>
{
    public Guid GroupId { get; set; }
}
public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, ResponseModel<string>>
{
    private readonly IGroupService _groupService;
    public DeleteGroupCommandHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    public async Task<ResponseModel<string>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupService.DeleteGroupAsync(request.GroupId, cancellationToken);
    }
}