using Microsoft.EntityFrameworkCore;
using Vorchestra.Domain.DataModels;

namespace Vochestra.Infrastructure;

public class VorchestraDbContext : DbContext
{
    public VorchestraDbContext(DbContextOptions<VorchestraDbContext> options) : base(options)
    {
    }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanSubscriptionHistory> PlanSubscriptionHistory { get; set; }

}
