using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TeamSpeak3QueryApi.Net.Specialized;
using TeamSpeak3QueryApi.Net.Specialized.Responses;
using TeamspeakAnalytics.hosting.Configuration;
using TeamspeakAnalytics.hosting.Contract;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting.Controllers
{
  [Route("api/server")]
  public class ServerController : BaseController
  {
    public ServerController(IConfiguration configuration, ITS3DataProvider ts3DataProvider,
      IOptions<TeamspeakConfiguration> ts3Config) : base(configuration, ts3DataProvider, ts3Config)
    {
    }

    [HttpGet("clients")]
    [ProducesResponseType(typeof(IEnumerable<GetClientDetailedInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClientsAsync(bool onlyVoiceClients = false)
    {
      IEnumerable<GetClientDetailedInfo> clients = await Ts3DataProvider.GetClientsDeatailedAsync();
      if (onlyVoiceClients)
        clients = clients?.Where(c => c.Type == ClientType.FullClient);
      return Ok(clients ?? new List<GetClientDetailedInfo>());
    }

    [HttpGet("clients/{clientDbId}")]
    [ProducesResponseType(typeof(IEnumerable<GetClientDetailedInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetClientAsync(int clientDbId)
    {
      if (clientDbId < 0)
        return BadRequest(nameof(clientDbId));

      var client = await Ts3DataProvider.GetGetClientDetailedInfoAsync(clientDbId);
      return Ok(client);
    }

    [HttpGet("channels")]
    [ProducesResponseType(typeof(IEnumerable<GetChannelListInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetChannelsAsync()
    {
      IEnumerable<GetChannelListInfo> channels = await Ts3DataProvider.GetChannelAsync();
      return Ok(channels ?? new List<GetChannelListInfo>());
    }

    [HttpGet("serverinfo")]
    [ProducesResponseType(typeof(GetServerListInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetServerListInfos()
    {
      var serverListInfos = await Ts3DataProvider.GetServerListInfosAsync();
      var serverInfo = serverListInfos?.FirstOrDefault(s => s.Id == TS3Config.Value.ServerIndex);
      if (serverInfo == null)
        return NoContent();

      return Ok(new DetailedTs3ServerInfo(serverInfo) { ExternalIPAddress = TS3Config.Value.ExternalAddress });
    }

    /// <summary>
    /// Test
    /// </summary>
    /// <param name="serverGroupType">TemplateGroup = 0, NormalGroup = 1, QueryGroup = 2</param>
    /// <returns></returns>
    [HttpGet("groups")]
    [ProducesResponseType(typeof(IEnumerable<GetServerGroupListInfo>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetServerGroups(ts3provider.Enums.ServerGroupType? serverGroupType = null)
    {
      IEnumerable<GetServerGroupListInfo> serverGroups = await Ts3DataProvider.GetServerGroups();

      if (serverGroupType.HasValue)
        serverGroups = serverGroups?.Where(g => g.ServerGroupType == (ServerGroupType) serverGroupType.Value);

      return Ok(serverGroups);
    }

    [HttpGet("groups/{groupId}/clients")]
    [ProducesResponseType(typeof(IEnumerable<GetServerGroupClientList>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetServerGroupClients(int groupId)
    {
      if (groupId < 0)
        return BadRequest(nameof(groupId));

      var clients = await Ts3DataProvider.GetServerGroupClients(groupId);
      return Ok(clients);
    }
  }
}