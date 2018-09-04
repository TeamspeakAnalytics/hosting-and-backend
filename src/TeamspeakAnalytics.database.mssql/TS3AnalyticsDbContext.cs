using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TeamspeakAnalytics.database.mssql.Entities;

namespace TeamspeakAnalytics.database.mssql
{
  public class TS3AnalyticsDbContext : DbContext
  {
    public TS3AnalyticsDbContext(DbContextOptions<TS3AnalyticsDbContext> options) : base(options)
    {

    }

    #region DbSets

    public DbSet<User> Users { get; set; }

    #endregion
  }
}
