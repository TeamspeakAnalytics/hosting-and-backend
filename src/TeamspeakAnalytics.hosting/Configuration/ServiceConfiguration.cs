using System;

namespace TeamspeakAnalytics.hosting.Configuration
{
  public class ServiceConfiguration
  {
    private int _port;
    private string _hostname;
    private TimeSpan _analyticsPeriod;
    private TimeSpan _aggregationPeriod;

    public string Hostname
    {
      get => _hostname;
      set
      {
        var hostnameType = Uri.CheckHostName(value);

        if (hostnameType == UriHostNameType.Unknown)
          throw new ArgumentException(nameof(Hostname), $"The given hostname ({value}) is not allowed");

        _hostname = value;
      }
    }

    public int Port
    {
      get => _port;
      set
      {
        if (value <= 0 || value > 65536)
          throw new ArgumentOutOfRangeException(nameof(Port),
            $"The given port ({value}) is not an allowed portNumber (1 - 65536)");

        _port = value;
      }
    }

    public bool UseHttps { get; set; }

    public string SecurityKey { get; set; }

    public TimeSpan AnalyticsPeriod
    {
      get => _analyticsPeriod;
      set
      {
        if (value == TimeSpan.Zero)
          throw new ArgumentOutOfRangeException(nameof(AnalyticsPeriod),
            $"The given timespan ({value}) has to be at least 1 second");

        _analyticsPeriod = value;
      }
    }

    public TimeSpan AggregationPeriod
    {
      get => _aggregationPeriod;
      set
      {
        if (value == TimeSpan.Zero)
          throw new ArgumentOutOfRangeException(nameof(AggregationPeriod),
            $"The given timespan ({value}) has to be at least 1 second");

        _aggregationPeriod = value;
      }
    }
  }
}