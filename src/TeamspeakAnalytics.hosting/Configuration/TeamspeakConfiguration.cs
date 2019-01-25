using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting.Configuration
{
  public class TeamspeakConfiguration : TS3ServerInfo
  {
    public string ExternalAddress { get; set; } = "127.0.0.1";
  }
}