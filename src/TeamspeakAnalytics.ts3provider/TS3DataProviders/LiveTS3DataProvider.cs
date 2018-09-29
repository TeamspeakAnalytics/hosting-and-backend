using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Responses;

namespace TeamspeakAnalytics.ts3provider.TS3DataProviders
{
  public class LiveTS3DataProvider : ITS3DataProvider
  {
    private TS3ServerInfo _ts3ServerInfo;

    public LiveTS3DataProvider(TS3ServerInfo ts3ServerInfo)
    {
      _ts3ServerInfo = ts3ServerInfo ?? throw new ArgumentNullException(nameof(ts3ServerInfo));
      if (String.IsNullOrWhiteSpace(ts3ServerInfo.QueryPassword))
        throw new ArgumentNullException(nameof(ts3ServerInfo.QueryPassword));

      // inits the client
      CheckConnection(true);
    }

    public TeamSpeakClient TeamSpeakClient { get; private set; }

    private DateTime _lastReceonnectTry = DateTime.MinValue;
    private object _ts3ClientSyncRoot = new object();
    public bool CheckConnection(bool reconnect = false)
    {
      bool checkFunc() => (TeamSpeakClient?.Client?.Client?.Connected ?? false) && (TeamSpeakClient?.Client?.IsConnected ?? false);

      if (checkFunc())
        return true;

      if (!reconnect)
        return checkFunc();

      try
      {
        lock (_ts3ClientSyncRoot)
        {
          if (checkFunc())
            return true;

          if ((_lastReceonnectTry + _ts3ServerInfo.QueryReconnectTimeout) > DateTime.Now)
            return false;

          _lastReceonnectTry = DateTime.Now;
          TeamSpeakClient?.Dispose();
          TeamSpeakClient = new TeamSpeakClient(_ts3ServerInfo.QueryHostname, _ts3ServerInfo.QueryPort);
          TeamSpeakClient.ConnectAndInitConnection(_ts3ServerInfo).Wait();

        }
      }
      catch (Exception ex) when (ex is QueryException || ex is QueryProtocolException)
      {
        //TODO: LOG
        return false;
      }

      return checkFunc();
    }

    public Task<IReadOnlyList<GetChannelListInfo>> GetChannelAsync(bool forceReload = false)
    {
      return TeamSpeakClient.GetChannels();
    }

    public Task<IReadOnlyList<GetClientInfo>> GetClientsAsync(bool forceReload = false)
    {
      return TeamSpeakClient.GetClients();
    }

    public async Task<GetClientDetailedInfo> GetGetClientDetailedInfoAsync(int clientDbId)
    {
      IEnumerable<GetClientInfo> clients = await GetClientsAsync();
      var client = clients.FirstOrDefault(c => c.DatabaseId == clientDbId);

      if (client == null)
        return null;

      return await TeamSpeakClient.GetClientInfo(client);
    }

    public async Task<IReadOnlyList<GetClientDetailedInfo>> GetClientsDeatailedAsync(bool forceReload = false)
    {
      var clients = await GetClientsAsync(true);
      var clientsFiltered = clients.Where(cl => cl.Type == ClientType.FullClient).DistinctBy(x => x.DatabaseId).ToList();
      var detailedList = new List<GetClientDetailedInfo>();

      if ((clientsFiltered?.Count ?? 0) < 1)
        return detailedList;

      foreach (var c in clientsFiltered)
      {
        var detailed = await TeamSpeakClient.GetClientInfo(c);
        detailedList.Add(detailed);
      }
      return detailedList;
    }
    
    public Task<IReadOnlyList<GetServerGroupListInfo>> GetServerGroups(bool forceReload = false)
    {
      return TeamSpeakClient.GetServerGroups();
    }

    public Task<IReadOnlyList<GetServerListInfo>> GetServerListInfosAsync(bool forceReload = false)
    {
      return TeamSpeakClient.GetServers();
    }

    public Task<IReadOnlyList<GetServerGroupClientList>> GetServerGroupClients(int serverGroupDatabaseId)
    {
      return TeamSpeakClient.GetServerGroupClientList(serverGroupDatabaseId);
    }

    public Task<IReadOnlyList<GetServerGroupClientList>> GetServerGroupClients(GetServerGroupListInfo serverGroup) => GetServerGroupClients(serverGroup.Id);

  }
}
