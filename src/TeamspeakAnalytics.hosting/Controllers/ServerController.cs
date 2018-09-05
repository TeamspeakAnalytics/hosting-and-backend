using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TeamSpeak3QueryApi.Net.Specialized.Responses;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting.Controllers
{
  [Route("api/server")]
  public class ServerController : BaseController
  {
    public ServerController(IConfiguration configuration, ITS3DataProvider ts3DataProvider, TS3ServerInfo tS3ServerInfo) : base(configuration, ts3DataProvider, tS3ServerInfo)
    {
    }

    [HttpGet("clients")]
    [ProducesResponseType(typeof(IEnumerable<GetClientInfo>), 200)]
    public async Task<IActionResult> GetClientsAsync(bool onlyVoiceClients = false)
    {
      IEnumerable<GetClientInfo> clients = await Ts3DataProvider.GetClientsAsync();
      if (onlyVoiceClients)
        clients = clients?.Where(c => c.Type == TeamSpeak3QueryApi.Net.Specialized.ClientType.FullClient);
      return Ok(clients ?? new List<GetClientInfo>());
    }

    [HttpGet("channels")]
    [ProducesResponseType(typeof(IEnumerable<GetChannelListInfo>), 200)]
    public async Task<IActionResult> GetChannelsAsync()
    {
      IEnumerable<GetChannelListInfo> channels = await Ts3DataProvider.GetChannelAsync();
      return Ok(channels ?? new List<GetChannelListInfo>());
    }

    [HttpGet("serverinfo")]
    [ProducesResponseType(typeof(GetServerListInfo), 200)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> GetServerListInfos()
    {
      var serverlistinfos = await Ts3DataProvider.GetServerListInfosAsync();
      return Ok(serverlistinfos?.FirstOrDefault(s => s.Id == TS3ServerInfo.ServerIndex));
    }
  }
}