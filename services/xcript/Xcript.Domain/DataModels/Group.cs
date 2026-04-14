using Shared.Domain.DataModels;

namespace Xcript.Domain.DataModels;

public class Group : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
}
