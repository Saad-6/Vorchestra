using Microsoft.EntityFrameworkCore;
using Shared.Application.Contracts;
using Xcript.Application.Commands.Script;
using Xcript.Application.Interfaces;
using Xcript.Infrastructure;
using Xcript.Infrastructure.Services;

namespace Xcript.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<XcriptDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(XcriptDbContext).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateScriptCommand).Assembly);
        });

        services.AddSingleton<IDatabaseService, DatabaseService>();

        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IScriptGroupService, ScriptGroupService>();
        services.AddScoped<IScriptService, ScriptService>();
        services.AddScoped<IScriptVariableService, ScriptVariableService>();
        services.AddScoped<IVariableService, VariableService>();
        services.AddScoped<IVariableSourceService, VariableSourceService>();

        return services;
    }
}
