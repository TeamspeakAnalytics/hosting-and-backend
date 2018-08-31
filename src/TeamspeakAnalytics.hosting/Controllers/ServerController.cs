using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamSpeak3QueryApi.Net.Specialized.Responses;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting.Controllers
{
  [Route("api/server")]
  public class ServerController : BaseController
  {
    public ServerController(ITS3DataProvider ts3DataProvider, TS3ServerInfo tS3ServerInfo) : base(ts3DataProvider, tS3ServerInfo)
    {
    }

    [HttpGet("clients")]
    [ProducesResponseType(typeof(IEnumerable<GetClientInfo>), 200)]
    public async Task<IActionResult> GetClientsAsync()
    {
      var clients = await Ts3DataProvider.GetClientsAsync();
      return Ok(clients ?? new List<GetClientInfo>());
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