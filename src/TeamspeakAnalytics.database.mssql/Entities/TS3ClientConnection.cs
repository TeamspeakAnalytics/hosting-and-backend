using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeamspeakAnalytics.database.mssql.Entities
{
  public class TS3ClientConnection
  {
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(TS3Client))]
    public Guid ClientGuid { get; set; }

    public DateTime TimeStampStart { get; set; }

    public DateTime TimeStampEnd { get; set; }

    public int ChannelId { get; set; }

    public TimeSpan IncactiveSince { get; set; }

    #region NavigationProperties

    public virtual TS3Client TS3Client { get; set; }

    #endregion
  }
}