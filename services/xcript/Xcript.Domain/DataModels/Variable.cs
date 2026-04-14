using Shared.Domain.DataModels;

namespace Xcript.Domain.DataModels;

public class Variable : BaseEntity
{
    public string Name { get; set; }
    public string Source { get; set; }
    public string Description { get; set; } 
}
