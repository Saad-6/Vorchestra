using Microsoft.EntityFrameworkCore;
using Vbaton.Domain.DataModels;

namespace Vbaton.Infrastructure;

public class VbatonDbContext : DbContext
{
    public VbatonDbContext(DbContextOptions<VbatonDbContext> options) : base(options)
    {
    }
    public DbSet<ExecutionLog> ExecutionLogs{ get; set; }
    public DbSet<ExecutionLogDetail> ExecutionLogDetails { get; set; }
}
