using Microsoft.EntityFrameworkCore;
using Workflow.Domain.DataModels;

namespace Workflow.Infrastructure;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }
    public DbSet<ServerWorkflow> ServerWorkflows { get; set; }
    public DbSet<ProjectWorkflow> ProjectWorkflows { get; set; }
    public DbSet<WorkflowGroup> WorkflowGroups { get; set; }
}
