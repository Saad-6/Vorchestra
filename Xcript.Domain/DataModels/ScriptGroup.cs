using Shared.Domain.DataModels;

namespace Xcript.Domain.DataModels;

public class ScriptGroup : BaseEntity
{
    public Guid ScriptId { get; set; }
    public Guid GroupId { get; set; }
    public int Order { get; set; }
}
