using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Contracts;
using Shared.Contracts.RequestModels;
using Shared.Domain.Constants;
using Vbaton.Application.Commands;
using Vbaton.Application.Interfaces;
using Vbaton.Application.Producers;
using Vbaton.Infrastructure;
using Vbaton.Infrastructure.EventHandlers;
using Vbaton.Infrastructure.EventPublishers;
using Vbaton.Infrastructure.Services;

namespace Vbarton.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<VbatonDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(ExecuteWorkFlowCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(VbatonDbContext).Assembly);
        });

        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddScoped<ISshService, SshService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IScriptEventPublisher, ScriptEventPublisher>();
        services.AddScoped<IProjectEventPublisher, ProjectEventPublisher>();

        services.AddMassTransit(x =>
        {
            x.AddRequestClient<ScriptsByIdsRequest>(new Uri($"queue:{Queues.Script.ScriptsByIds}"));
            x.AddRequestClient<ScriptsByGroupIdRequest>(new Uri($"queue:{Queues.Script.ScriptsByGroupId}"));
            x.AddRequestClient<ProjectByIdRequest>(new Uri($"queue:{Queues.Project.ProjectById}"));

            x.AddConsumer<ExecuteWorkFlowEventHandler>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint(Queues.WorkFlow.ExecuteWorkFlow, e =>
                {
                    e.ConfigureConsumer<ExecuteWorkFlowEventHandler>(context);
                });
            });
        });

        return services;
    }
}
