using System.Security.Cryptography;
using Shared.Application.Interfaces;

namespace Shared.Infrastructure.Services;

public class HashService : IHashService
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public Task<string> HashAsync(string input, CancellationToken cancellationToken = default)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize);
        var result = $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        return Task.FromResult(result);
    }

    public Task<bool> VerifyAsync(string input, string hash, CancellationToken cancellationToken = default)
    {
        var parts = hash.Split('.');
        if (parts.Length != 2) return Task.FromResult(false);

        var salt = Convert.FromBase64String(parts[0]);
        var expectedKey = Convert.FromBase64String(parts[1]);
        var actualKey = Rfc2898DeriveBytes.Pbkdf2(input, salt, Iterations, Algorithm, KeySize);

        return Task.FromResult(CryptographicOperations.FixedTimeEquals(actualKey, expectedKey));
    }
}
