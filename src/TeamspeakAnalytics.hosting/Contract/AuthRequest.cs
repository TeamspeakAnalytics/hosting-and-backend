using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TeamspeakAnalytics.hosting.Contract
{
  public class AuthRequest
  {
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
  }
}