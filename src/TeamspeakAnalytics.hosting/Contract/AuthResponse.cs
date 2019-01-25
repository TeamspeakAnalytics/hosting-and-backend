using System.IdentityModel.Tokens.Jwt;

namespace TeamspeakAnalytics.hosting.Contract
{
  public class AuthResponse
  {
    private string _token;

    public AuthResponse(JwtSecurityToken token)
    {
      Token = new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string Token
    {
      get => _token;
      set => _token = value;
    }

    public string BearerString
    {
      get => $"Bearer {_token}";
      private set => _token = value;
    }
  }
}