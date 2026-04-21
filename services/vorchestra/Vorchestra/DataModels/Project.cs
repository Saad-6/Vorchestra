using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class Project : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string? Url { get; set; }
    public bool IsRelative { get; set; }
    public string? Branch { get; set; }
    public string? PersonalAccessToken { get; set; }
    public string? ZipFilePath { get; set; }
}
