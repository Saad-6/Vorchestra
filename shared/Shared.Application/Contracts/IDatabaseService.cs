namespace Shared.Application.Contracts;

public interface IDatabaseService
{
    Task MigrateDatabaseAsync();
    Task SeedDefaultDataAsync();
}
