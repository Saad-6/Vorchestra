using Shared.Domain.DataModels;

namespace Xcript.Domain.DataModels;

public class Script : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
}
