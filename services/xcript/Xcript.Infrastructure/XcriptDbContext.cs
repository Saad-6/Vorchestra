using Microsoft.EntityFrameworkCore;
using Xcript.Domain.DataModels;

namespace Xcript.Infrastructure;

public class XcriptDbContext : DbContext
{
    public XcriptDbContext(DbContextOptions<XcriptDbContext> options) : base(options)
    {
    }
    public DbSet<Script> Scripts { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<ScriptGroup> ScriptGroups { get; set; }
    public DbSet<Variable> Variables { get; set; }
    public DbSet<ScriptVariable> ScriptVariables { get; set; }
}
