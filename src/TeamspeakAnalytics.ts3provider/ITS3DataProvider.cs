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

    Task<GetClientDetailedInfo> GetGetClientDetailedInfoAsync(int clientDbId);

    Task<IReadOnlyList<GetClientDetailedInfo>> GetClientsDeatailedAsync(bool forceReload = false);

    Task<IReadOnlyList<GetChannelListInfo>> GetChannelAsync(bool forceReload = false);

    Task<IReadOnlyList<GetServerListInfo>> GetServerListInfosAsync(bool forceReload = false);

    Task<IReadOnlyList<GetServerGroupListInfo>> GetServerGroups(bool forceReload = false);

    Task<IReadOnlyList<GetServerGroupClientList>> GetServerGroupClients(int serverGroupDatabaseId);

    Task<IReadOnlyList<GetServerGroupClientList>> GetServerGroupClients(GetServerGroupListInfo serverGroup);

    bool CheckConnection(bool reconnect = false);
  }
}