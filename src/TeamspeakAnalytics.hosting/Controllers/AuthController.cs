using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TeamspeakAnalytics.hosting.Configuration;
using TeamspeakAnalytics.hosting.Contract;
using TeamspeakAnalytics.ts3provider;
using TeamspeakAnalytics.hosting.Helper;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TeamspeakAnalytics.database.mssql;

namespace TeamspeakAnalytics.hosting.Controllers
{
  [Route("api/auth")]
  public class AuthController : BaseController
  {
    private readonly TS3AnalyticsDbContext _ts3AnalyticsDbContext;
    private readonly IOptions<ServiceConfiguration> _serviceConfiguration;

    public AuthController(TS3AnalyticsDbContext ts3AnalyticsDbContext, IConfiguration configuration, ITS3DataProvider ts3DataProvider,
      IOptions<TeamspeakConfiguration> ts3Config, IOptions<ServiceConfiguration> serviceConfiguration) : base(configuration, ts3DataProvider, ts3Config)
    {
      _ts3AnalyticsDbContext = ts3AnalyticsDbContext;
      _serviceConfiguration = serviceConfiguration;
    }

    [AllowAnonymous]
    [HttpPost("requestjwttoken")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public IActionResult RequestJwtToken([FromBody] AuthRequest authRequest)
    {
      var cfg = _serviceConfiguration.Value;
      if (authRequest == null || !CheckAuth(authRequest))
        return BadRequest("Could not authenticate");

      var claims = new[]
      {
        new Claim(ClaimTypes.Name, authRequest.Username)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.SecurityKey));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        issuer: cfg.Hostname,
        audience: cfg.Hostname,
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: credentials);

      return Ok(new AuthResponse(token));
    }

    private bool CheckAuth([NotNull] AuthRequest authRequest)
    {
      //TODO: AM: implement auth with database comparison
      var db = _ts3AnalyticsDbContext;
      return true;
    }
  }
}