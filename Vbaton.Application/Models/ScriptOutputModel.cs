namespace Vbaton.Application.Models;

public class ScriptOutputModel
{
    public Guid ScriptId { get; set; }
    public string Output { get; set; }
    public bool Succeeded { get; set; }
}
