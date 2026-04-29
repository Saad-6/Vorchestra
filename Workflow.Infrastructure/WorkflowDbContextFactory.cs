using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Workflow.Infrastructure;

public class WorkflowDbContextFactory : IDesignTimeDbContextFactory<WorkflowDbContext>
{
    public WorkflowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorkflowDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=vorchestra;Username=postgres;Password=postgres");
        return new WorkflowDbContext(optionsBuilder.Options);
    }
}
