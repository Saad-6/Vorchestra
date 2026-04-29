using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Contracts;

namespace Workflow.Infrastructure.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task MigrateDatabaseAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        await context.Database.MigrateAsync();
    }

    public async Task SeedDefaultDataAsync()
    {
        await Task.CompletedTask;
    }
}
