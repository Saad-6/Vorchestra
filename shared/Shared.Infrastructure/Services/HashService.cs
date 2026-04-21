using Shared.Application.Interfaces;

namespace Shared.Infrastructure.Services;

public class HashService : IHashService
{
    public Task<string> HashAsync(string input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> VerifyAsync(string input, string hash, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
