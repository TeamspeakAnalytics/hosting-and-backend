using Microsoft.EntityFrameworkCore;
using TeamspeakAnalytics.database.mssql.Entities;

namespace TeamspeakAnalytics.database.mssql
{
  public class TS3AnalyticsDbContext : DbContext
  {
    public TS3AnalyticsDbContext(DbContextOptions<TS3AnalyticsDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasIndex(i => i.Username)
        .IsUnique(true);

      modelBuilder.Entity<TS3Client>()
        .HasIndex(i => i.UniqueIdentifier)
        .IsUnique(true);

      base.OnModelCreating(modelBuilder);
    }

    #region DbSets

    public DbSet<User> Users { get; set; }

    public DbSet<TS3Client> TS3Clients { get; set; }

    public DbSet<TS3ClientConnection> TS3ClientConnection { get; set; }

    #endregion
  }
}
