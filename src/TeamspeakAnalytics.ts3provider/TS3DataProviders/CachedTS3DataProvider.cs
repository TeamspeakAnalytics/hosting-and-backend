using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Responses;

namespace TeamspeakAnalytics.ts3provider
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
      if (String.IsNullOrWhiteSpace(ts3ServerInfo.Password))
        throw new ArgumentNullException(nameof(ts3ServerInfo.Password));
      
      CheckConnection(true);

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
      _serverList = new UpdateableInfo<IReadOnlyList<GetServerListInfo>>(this, (tsc) => tsc.GetServers())
      {
        UpdatePeriod = new TimeSpan(0, 1, 0),
        AutoUpdate = false,
      };
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

          if ((_lastReceonnectTry + _ts3ServerInfo.ReconnectTimeout) > DateTime.Now)
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

    private UpdateableInfo<IReadOnlyList<GetServerListInfo>> _serverList;

    public async Task<IReadOnlyList<GetServerListInfo>> GetServerListInfosAsync(bool forceReload = false)
    {
      if (forceReload)
        await _serverList.UpdateAsync();

      return await _serverList.GetValueAsync();
    }

    #endregion
  }
}
