using TeamSpeak3QueryApi.Net.Specialized.Responses;

namespace TeamspeakAnalytics.hosting.Contract
{
  public class DetailedTs3ServerInfo : GetServerListInfo
  {
    public DetailedTs3ServerInfo(GetServerListInfo serverInfo)
    {
      Autostart = serverInfo.Autostart;
      ClientsOnline = serverInfo.ClientsOnline;
      Id = serverInfo.Id;
      MachineId = serverInfo.MachineId;
      MaxClients = serverInfo.MaxClients;
      Name = serverInfo.Name;
      Port = serverInfo.Port;
      QueriesOnline = serverInfo.QueriesOnline;
      Status = serverInfo.Status;
      Uid = serverInfo.Uid;
      Uptime = serverInfo.Uptime;
    }

    public string ExternalIPAdress { get; set; }
  }
}
