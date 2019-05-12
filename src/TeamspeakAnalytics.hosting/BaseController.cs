using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TeamspeakAnalytics.hosting.Configuration;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting
{
  public class BaseController : Controller
  {
    protected readonly ITS3DataProvider Ts3DataProvider;
    protected readonly IOptions<TeamspeakConfiguration> TS3Config;
    protected readonly IConfiguration Configuration;

    public BaseController(IConfiguration configuration, ITS3DataProvider ts3DataProvider,
      IOptions<TeamspeakConfiguration> ts3Config)
    {
      Configuration = configuration;
      Ts3DataProvider = ts3DataProvider;
      TS3Config = ts3Config;
    }
  }
}