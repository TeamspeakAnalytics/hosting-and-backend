using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using TeamspeakAnalytics.hosting.Configuration;
using TeamspeakAnalytics.hosting.Helper;

namespace TeamspeakAnalytics.hosting
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
      var configUrl = BuildUrl();

      return WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseUrls(configUrl)
        .Build();
    }

    public static string BuildUrl()
    {
      var config = new ConfigurationBuilder()
        .SetBasePath(
          //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
          Directory.GetCurrentDirectory()
        )
        .AddJsonFile("appsettings.json")
        .Build()
        .GetSection<ServiceConfiguration>();

      var httpPrefix = config.UseHttps ? "https" : "http";

      var url = $"{httpPrefix}://{config.Hostname}:{config.Port}";
      return url;
    }
  }
}
