namespace Vorchestra.Application.Interfaces;

public interface IDatabaseService
{
    Task MigrateDatabaseAsync();
    Task SeedDefaultDataAsync();
}
