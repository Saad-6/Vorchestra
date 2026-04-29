using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vochestra.Infrastructure;

public class VorchestraDbContextFactory : IDesignTimeDbContextFactory<VorchestraDbContext>
{
    public VorchestraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VorchestraDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=vorchestra;Username=postgres;Password=postgres");
        return new VorchestraDbContext(optionsBuilder.Options);
    }
}
