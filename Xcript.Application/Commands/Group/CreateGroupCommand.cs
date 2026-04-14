using MediatR;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.DTOs;

namespace Xcript.Application.Commands.Group;

public class CreateGroupCommand : CreateGroupDto, IRequest<ResponseModel<Guid>>
{
}
public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, ResponseModel<Guid>>
{
    private readonly IGroupService _groupService;
    public CreateGroupCommandHandler(IGroupService groupService)
    {
        _groupService = groupService;
    }
    public async Task<ResponseModel<Guid>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        return await _groupService.CreateGroupAsync(request, cancellationToken);
    }
}
