using Microsoft.EntityFrameworkCore;
using Shared.Application.Contracts;
using Vochestra.Infrastructure;
using Vochestra.Infrastructure.Services;
using Vorchestra.Application.Commands.Plan;
using Vorchestra.Application.Interfaces;

namespace Vorchestra.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<VorchestraDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(VorchestraDbContext).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreatePlanCommand).Assembly);
        });

        services.AddSingleton<IDatabaseService, DatabaseService>();

        services.AddScoped<IServerService, ServerService>();

        services.AddScoped<ITenantService, TenantService>();

        services.AddScoped<IProjectService, ProjectService>();

        services.AddScoped<IPlanService, PlanService>();

        return services;
    }
}
