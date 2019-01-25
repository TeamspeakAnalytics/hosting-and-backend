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

namespace TeamspeakAnalytics.hosting.Controllers
{
  [Route("api/auth")]
  public class AuthController : BaseController
  {
    public AuthController(IConfiguration configuration, ITS3DataProvider ts3DataProvider,
      TeamspeakConfiguration ts3Config) : base(configuration, ts3DataProvider, ts3Config)
    {
    }

    [AllowAnonymous]
    [HttpPost("requestjwttoken")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public IActionResult RequestJwtToken([FromBody] AuthRequest authRequest)
    {
      var cfg = Configuration.GetSection<ServiceConfiguration>();
      if (authRequest == null || !CheckAuth(authRequest))
        return BadRequest("Could not authenticate");

      var claims = new[]
      {
        new Claim(ClaimTypes.Name, authRequest.Username)
      };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.SecurityKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
        issuer: cfg.Hostname,
        audience: cfg.Hostname,
        claims: claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: creds);

      return Ok(new AuthResponse(token));
    }

    private bool CheckAuth([NotNull] AuthRequest authRequest)
    {
      return true;
    }
  }
}