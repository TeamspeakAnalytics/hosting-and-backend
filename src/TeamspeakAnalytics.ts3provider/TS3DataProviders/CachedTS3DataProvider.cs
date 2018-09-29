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
  public class CachedTS3DataProvider : ITS3DataProvider
  {
    #region Fields

    private readonly TS3ServerInfo _ts3ServerInfo;

    #endregion

    #region Ctor

    public CachedTS3DataProvider(TS3ServerInfo ts3ServerInfo)
    {
      _ts3ServerInfo = ts3ServerInfo ?? throw new ArgumentNullException(nameof(ts3ServerInfo));
      if (String.IsNullOrWhiteSpace(ts3ServerInfo.QueryPassword))
        throw new ArgumentNullException(nameof(ts3ServerInfo.QueryPassword));

      CheckConnection(true);

      // inits the client
      InitUpdateableInfo();
    }

    #endregion

    private void InitUpdateableInfo()
    {
      _clients = new UpdateableInfo<IReadOnlyList<GetClientInfo>>(this, (tsc) => tsc.GetClients())
      {
        UpdatePeriod = new TimeSpan(0, 1, 0),
        AutoUpdate = false,
      };
      _clientsDetailed = new UpdateableInfo<IReadOnlyList<GetClientDetailedInfo>>(this, (tsc) => getClientsDetailedTaskAsync(tsc))
      {
        UpdatePeriod = new TimeSpan(0, 1, 0),
        AutoUpdate = false,
      };
      _channel = new UpdateableInfo<IReadOnlyList<GetChannelListInfo>>(this, (tsc) => tsc.GetChannels())
      {
        UpdatePeriod = new TimeSpan(0, 1, 0),
        AutoUpdate = false,
      };
      _serverList = new UpdateableInfo<IReadOnlyList<GetServerListInfo>>(this, (tsc) => tsc.GetServers())
      {
        UpdatePeriod = new TimeSpan(0, 1, 0),
        AutoUpdate = false,
      };
      _serverGroups = new UpdateableInfo<IReadOnlyList<GetServerGroupListInfo>>(this, (tsc) => tsc.GetServerGroups())
      {
        UpdatePeriod = new TimeSpan(0, 1, 0),
        AutoUpdate = false,
      };
    }

    private async Task<IReadOnlyList<GetClientDetailedInfo>> getClientsDetailedTaskAsync(TeamSpeakClient tsc)
    {
      var clients = await GetClientsAsync(true);
      var clientsFiltered = clients.Where(cl => cl.Type == ClientType.FullClient).DistinctBy(x => x.DatabaseId).ToList();
      var detailedList = new List<GetClientDetailedInfo>();

      if ((clientsFiltered?.Count ?? 0) < 1)
        return detailedList;

      foreach (var c in clientsFiltered)
      {
        var detailed = await tsc.GetClientInfo(c);
        detailedList.Add(detailed);
      }
      return detailedList;
    }

    #region ITS3DataProvider

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

    private UpdateableInfo<IReadOnlyList<GetClientInfo>> _clients;
    public async Task<IReadOnlyList<GetClientInfo>> GetClientsAsync(bool forceReload = false)
    {
      if (forceReload)
        await _clients.UpdateAsync();

      return await _clients.GetValueAsync();
    }

    private UpdateableInfo<IReadOnlyList<GetClientDetailedInfo>> _clientsDetailed;
    public async Task<IReadOnlyList<GetClientDetailedInfo>> GetClientsDeatailedAsync(bool forceReload = false)
    {
      if (forceReload)
        await _clientsDetailed.UpdateAsync();

      return await _clientsDetailed.GetValueAsync();
    }

    private UpdateableInfo<IReadOnlyList<GetChannelListInfo>> _channel;
    public async Task<IReadOnlyList<GetChannelListInfo>> GetChannelAsync(bool forceReload = false)
    {
      if (forceReload)
        await _channel.UpdateAsync();

      return await _channel.GetValueAsync();
    }

    private UpdateableInfo<IReadOnlyList<GetServerListInfo>> _serverList;
    public async Task<IReadOnlyList<GetServerListInfo>> GetServerListInfosAsync(bool forceReload = false)
    {
      if (forceReload)
        await _serverList.UpdateAsync();

      return await _serverList.GetValueAsync();
    }

    private UpdateableInfo<IReadOnlyList<GetServerGroupListInfo>> _serverGroups;
    public async Task<IReadOnlyList<GetServerGroupListInfo>> GetServerGroups(bool forceReload = false)
    {
      if (forceReload)
        await _serverGroups.UpdateAsync();

      return await _serverGroups.GetValueAsync();
    }

    public Task<IReadOnlyList<GetServerGroupClientList>> GetServerGroupClients(int serverGroupDatabaseId)
    {
      throw new NotImplementedException();
    }

    public Task<IReadOnlyList<GetServerGroupClientList>> GetServerGroupClients(GetServerGroupListInfo serverGroup) => GetServerGroupClients(serverGroup.Id);

    public Task<GetClientDetailedInfo> GetGetClientDetailedInfoAsync(int clientDbId)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
