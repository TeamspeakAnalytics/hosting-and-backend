using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeamspeakAnalytics.database.mssql.Entities
{
  public class TS3Client
  {
    [Key]
    public Guid Id { get; set; }

    [Required, StringLength(28)]
    public string UniqueIdentifier { get; set; }

    public int DatabaseId { get; set; }

    public string NickName { get; set; }

    public string TS3Version { get; set; }

    public string TS3Plattform { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastConnected { get; set; }

    public int TotalConnectionCount { get; set; }

    public DateTime ChangeDate { get; set; }


    #region NavigationProperties

    public virtual ICollection<TS3ClientConnection> TS3ClientConnections { get; set; }

    #endregion
  }
}
