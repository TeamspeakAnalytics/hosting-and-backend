using Microsoft.AspNetCore.Mvc;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting
{
  public class BaseController : Controller
  {
    protected readonly ITS3DataProvider Ts3DataProvider;
    protected readonly TS3ServerInfo TS3ServerInfo;

    public BaseController(ITS3DataProvider ts3DataProvider, TS3ServerInfo tS3ServerInfo)
    {
      Ts3DataProvider = ts3DataProvider;
      TS3ServerInfo = tS3ServerInfo;
    }
  }
}
