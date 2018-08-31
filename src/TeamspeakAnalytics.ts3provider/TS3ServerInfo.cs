using System;

namespace TeamspeakAnalytics.ts3provider
{
  public class TS3ServerInfo
  {
    public string Username { get; set; } = "serveradmin";
    public string Password { get; set; }
    public string QueryHostname { get; set; } = "127.0.0.1";
    public int QueryPort { get; set; } = 10011;
    public int ServerIndex { get; set; } = 1;

    public TimeSpan ReconnectTimeout { get; set; } = new TimeSpan(0, 1, 0);
  }
}
