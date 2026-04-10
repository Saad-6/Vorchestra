using Shared.Domain.DataModels;

namespace Xcript.Domain.DataModels;

public class ScriptVariable : BaseEntity
{
    public Guid ScriptId { get; set; }
    public Guid VariableId { get; set; }
}
