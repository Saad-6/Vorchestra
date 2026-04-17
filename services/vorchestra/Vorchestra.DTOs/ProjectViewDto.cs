namespace Vorchestra.DTOs;

public class ProjectViewDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Url { get; set; }
    public bool IsRelative { get; set; }
    public string? Branch { get; set; }
    public string? ZipFilePath { get; set; }
}
