using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Contracts;
using Shared.Domain.Constants;
using Workflow.Application.Commands;
using Workflow.Application.Interfaces;
using Workflow.Infrastructure;
using Workflow.Infrastructure.EventHandlers;
using Workflow.Infrastructure.Services;

namespace Workflow.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<WorkflowDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(WorkflowDbContext).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(CreateProjectWorkflowCommand).Assembly);
        });

        services.AddSingleton<IDatabaseService, DatabaseService>();

        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IWorkflowGroupService, WorkflowGroupService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProjectWorkflowsByTriggerEventHandler>();
            x.AddConsumer<ServerWorkflowsByTriggerEventHandler>();
            x.AddConsumer<ServerWorkflowByIdEventHandler>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint(Queues.Workflow.ProjectWorkflowsByTrigger, e =>
                {
                    e.ConfigureConsumer<ProjectWorkflowsByTriggerEventHandler>(context);
                });

                cfg.ReceiveEndpoint(Queues.Workflow.ServerWorkflowsByTrigger, e =>
                {
                    e.ConfigureConsumer<ServerWorkflowsByTriggerEventHandler>(context);
                });

                cfg.ReceiveEndpoint(Queues.Workflow.ServerWorkflowById, e =>
                {
                    e.ConfigureConsumer<ServerWorkflowByIdEventHandler>(context);
                });
            });
        });

        return services;
    }
}
