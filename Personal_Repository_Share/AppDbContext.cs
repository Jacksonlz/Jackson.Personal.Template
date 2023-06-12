using Microsoft.EntityFrameworkCore;

namespace Personal.Repository.Share;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //add decimal range
        configurationBuilder.Properties<decimal>()
          .HavePrecision(18, 10);
    }
}