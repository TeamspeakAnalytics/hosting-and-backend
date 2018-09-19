using System.Collections.Generic;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Responses;

namespace TeamspeakAnalytics.ts3provider
{
  public interface ITS3DataProvider
  {
    TeamSpeakClient TeamSpeakClient { get; }

    Task<IReadOnlyList<GetClientInfo>> GetClientsAsync(bool forceReload = false);

    Task<IReadOnlyList<GetClientDetailedInfo>> GetClientsDeatailedAsync(bool forceReload = false);

    Task<IReadOnlyList<GetChannelListInfo>> GetChannelAsync(bool forceReload = false);

    Task<IReadOnlyList<GetServerListInfo>> GetServerListInfosAsync(bool forceReload = false);

    bool CheckConnection(bool reconnect = false);
  }
}