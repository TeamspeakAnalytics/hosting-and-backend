using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting
{
  public class BaseController : Controller
  {
    protected readonly ITS3DataProvider Ts3DataProvider;
    protected readonly TS3ServerInfo TS3ServerInfo;
    protected readonly IConfiguration Configuration;

    public BaseController(IConfiguration configuration, ITS3DataProvider ts3DataProvider, TS3ServerInfo tS3ServerInfo)
    {
      Configuration = configuration;
      Ts3DataProvider = ts3DataProvider;
      TS3ServerInfo = tS3ServerInfo;
    }

  }
}
