namespace Shared.Contracts.ResponseModels;

public class ProjectByIdResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Source { get; set; } // Path or Url
    public bool IsSourceControl { get; set; } = false;
    public string? PersonalAccessToken { get; set; } = null;
}
