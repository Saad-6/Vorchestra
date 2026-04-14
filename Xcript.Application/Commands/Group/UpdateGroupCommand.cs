using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Commands.Group;

public class UpdateGroupCommand : UpdateGroupDto, IRequest<ResponseModel<Guid>>
{
}
public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, ResponseModel<Guid>>
{
    private readonly IGroupService _groupService;
    public UpdateGroupCommandHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    public async Task<ResponseModel<Guid>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupService.UpdateGroupAsync(request, cancellationToken);
    }
}
