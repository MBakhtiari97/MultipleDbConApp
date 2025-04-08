using Core;
using Microsoft.EntityFrameworkCore;

namespace DataLayer;

public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }
    public DbSet<SystemLog> SystemLog { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<AppUser>();
    }
}