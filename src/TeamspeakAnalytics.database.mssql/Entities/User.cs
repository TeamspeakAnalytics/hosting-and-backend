using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeamspeakAnalytics.database.mssql.Entities
{
  public class User
  {
    [Key]
    public int UserId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
  }
}
