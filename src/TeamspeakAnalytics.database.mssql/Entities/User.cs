using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamspeakAnalytics.database.mssql.Entities
{
  public class User
  {
    [Key]
    public Guid UserId { get; set; }

    [Required]
    public string Username { get; set; }

    public string Password { get; set; }

  }
}
