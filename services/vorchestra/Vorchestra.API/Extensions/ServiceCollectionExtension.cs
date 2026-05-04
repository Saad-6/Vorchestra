using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Contracts;
using Shared.Application.Interfaces;
using Shared.Contracts.RequestModels;
using Shared.Domain.Constants;
using Shared.DTO;
using Shared.Infrastructure.Services;
using Vochestra.Infrastructure;
using Vochestra.Infrastructure.Services;
using Vorchestra.Application.Commands.Plan;
using Vorchestra.Application.Interfaces;
using Vorchestra.Application.Producers;
using Vorchestra.Infrastructure.EventPublishers;

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
        services.AddSingleton<IHashService, HashService>();

        services.AddScoped<IServerService, ServerService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantProjectService, TenantProjectService>();
        services.AddScoped<ITenantSubscriptionService, TenantSubscriptionService>();
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IPlanService, PlanService>();

        services.AddScoped<ITenantEventPublisher, TenantEventPublisher>();
        services.AddScoped<IServerEventPublisher, ServerEventPublisher>();

        services.AddMassTransit(x =>
        {
            x.AddRequestClient<ProjectWorkflowsByTriggerRequest>(new Uri($"queue:{Queues.Workflow.ProjectWorkflowsByTrigger}"));
            x.AddRequestClient<ServerWorkflowsByTriggerRequest>(new Uri($"queue:{Queues.Workflow.ServerWorkflowsByTrigger}"));
            x.AddRequestClient<ServerWorkflowByIdRequest>(new Uri($"queue:{Queues.Workflow.ServerWorkflowById}"));
            x.AddRequestClient<ExecutionRequestDto>(new Uri($"queue:{Queues.WorkFlow.ExecuteWorkFlow}"));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
