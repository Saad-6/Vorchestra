namespace Shared.Application.Interfaces;

public interface IHashService
{
    Task<string> HashAsync(string input, CancellationToken cancellationToken = default);
    Task<bool> VerifyAsync(string input, string hash, CancellationToken cancellationToken = default);
}
