using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using TeamspeakAnalytics.database.mssql;
using TeamspeakAnalytics.hosting.Configuration;
using TeamspeakAnalytics.hosting.Helper;

namespace TeamspeakAnalytics.hosting
{
  public class Program
  {
    private static IConfiguration _configuration; 

    public static void Main(string[] args)
    {
      var webhost = BuildWebHost(args);
      InitDB();

      webhost.Run();
    }

    private static bool InitDB()
    {
      var dbContextOptionsBuilder = new DbContextOptionsBuilder<TS3AnalyticsDbContext>();
      dbContextOptionsBuilder.UseSqlServer(_configuration.GetConnectionString("ServiceDatabase"),
                            b => b.MigrationsAssembly("TeamspeakAnalytics.database.mssql"));

      using (var db = new TS3AnalyticsDbContext(dbContextOptionsBuilder.Options))
      {
        if (db.Database.GetPendingMigrations().Any())
        {
          //TODO: log updating database
          db.Database.Migrate();
        }
        else
        {
          //TODO; log no update needed.
        }
      }
      return true;
    }

    public  static IWebHost BuildWebHost(string[] args)
    {
      var configUrl = BuildUrl();

      return WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseUrls(configUrl)
        .Build();
    }

    private static string BuildUrl()
    {
      _configuration = new ConfigurationBuilder()
        .SetBasePath(
          //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
          Directory.GetCurrentDirectory()
        )
        .AddJsonFile("appsettings.json")
        .Build();

      var config = _configuration.GetSection<ServiceConfiguration>();

      var httpPrefix = config.UseHttps ? "https" : "http";

      var url = $"{httpPrefix}://{config.Hostname}:{config.Port}";
      return url;
    }
  }
}
