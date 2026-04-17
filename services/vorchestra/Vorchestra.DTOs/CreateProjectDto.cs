namespace Vorchestra.DTOs;

public class CreateProjectDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public bool IsRelative { get; set; }
    public string? Branch { get; set; }
    public string? PersonalAccessToken { get; set; }
}
