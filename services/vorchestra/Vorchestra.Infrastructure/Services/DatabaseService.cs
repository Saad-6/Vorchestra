using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Contracts;

namespace Vochestra.Infrastructure.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IServiceProvider _serviceProvider;
    public DatabaseService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task MigrateDatabaseAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VorchestraDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task SeedDefaultDataAsync()
    {
        // Implement seeding logic here if needed, e.g., creating default plans, servers, etc.
    }
}
