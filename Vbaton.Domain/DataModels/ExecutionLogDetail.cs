using Shared.Domain.DataModels;

namespace Vbaton.Domain.DataModels;

public class ExecutionLogDetail : BaseEntity
{
    public Guid ExecutionLogId { get; set; }
    public Guid ScriptId { get; set; }
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public string Output { get; set; }
}
