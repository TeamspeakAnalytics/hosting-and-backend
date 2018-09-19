using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeamspeakAnalytics.ts3provider;
using Microsoft.Extensions.DependencyInjection;
using TeamspeakAnalytics.database.mssql;
using System.Linq;
using TeamspeakAnalytics.database.mssql.Entities;
using TeamSpeak3QueryApi.Net.Specialized.Responses;
using System.Collections.Generic;
using TeamspeakAnalytics.hosting.Configuration;

namespace TeamspeakAnalytics.hosting
{
  public class AnalyticsJob : IHostedService, IDisposable
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly IServiceProvider _serviceProvider;
    private readonly ITS3DataProvider _ts3DataProvider;
    private readonly ServiceConfiguration _serviceConfiguration;
    private readonly TimeSpan _delay;
    private Task _executingJob;

    public AnalyticsJob(IServiceProvider serviceProvider, ITS3DataProvider ts3DataProvider, ServiceConfiguration serviceConfiguration)
    {
      _serviceProvider = serviceProvider;
      _ts3DataProvider = ts3DataProvider;
      _serviceConfiguration = serviceConfiguration;
      _delay = _serviceConfiguration.AnalyticsPeriod;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
      _executingJob = RunBackgroundJob(_cts.Token);

      if (_executingJob.IsCanceled)
        return _executingJob;

      return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
      if (_executingJob == null) return;

      try
      {
        _cts.Cancel();
      }
      finally
      {
        await Task.WhenAny(
          _executingJob,
          Task.Delay(Timeout.Infinite, cancellationToken)
        );
      }
    }

    public void Dispose()
    {
      _cts.Cancel();
    }

    private async Task RunBackgroundJob(CancellationToken ctx)
    {
      while (!ctx.IsCancellationRequested)
      {
        var timeStamp = DateTime.UtcNow;
        var nextRun = timeStamp.Add(_delay);
        var ts3Clients = await _ts3DataProvider.GetClientsDeatailedAsync(true);
        var ts3ClientsDbId = ts3Clients.Select(cl => cl.DatabaseId).ToList();

        using (var scope = _serviceProvider.CreateScope())
        using (var dbContext = scope.ServiceProvider.GetService<TS3AnalyticsDbContext>())
        {
          var dbClients = dbContext.TS3Clients.Where(dbtscl => ts3ClientsDbId.Contains(dbtscl.DatabaseId)).ToList();

          var clientTupleList = (from tsc in ts3Clients
                                join dbtsc in dbClients
                                on tsc.DatabaseId equals dbtsc.DatabaseId
                                into tsclmappings
                                from tsclmapping in tsclmappings.DefaultIfEmpty()
                                select analyzeClientTuple(tsc, tsclmapping, dbContext, timeStamp, nextRun))
                                .ToList();

          dbContext.SaveChanges();

          // Run job
          //Console.WriteLine($"{DateTime.Now:o}Test");
        }

        while (DateTime.UtcNow < nextRun)
        {
          await Task.Delay(500, ctx);
        }
      }
    }

    private static Tuple<GetClientDetailedInfo, TS3Client> analyzeClientTuple(
      GetClientDetailedInfo tscl, TS3Client dbcl, TS3AnalyticsDbContext dbContext, DateTime timeStamp, DateTime endTime)
    {
      if (dbcl == null)
      {
        dbcl = new TS3Client
        {
          Created = timeStamp,
          DatabaseId = tscl.DatabaseId,
          Id = Guid.NewGuid(),
          UniqueIdentifier = tscl.UniqueIdentifier,
          TS3ClientConnections = new List<TS3ClientConnection>()
        };
        dbContext.TS3Clients.Add(dbcl);
      }

      dbcl.ChangeDate = timeStamp;
      dbcl.LastConnected = timeStamp;
      dbcl.NickName = tscl.NickName;
      dbcl.TotalConnectionCount = tscl.TotalConnectionCount;
      dbcl.TS3Plattform = tscl.Plattform;
      dbcl.TS3Version = tscl.Version;

      dbContext.TS3ClientConnection.Add(new TS3ClientConnection
      {
        ChannelId = tscl.ChannelId,
        ClientGuid = dbcl.Id,
        Id = Guid.NewGuid(),
        IncactiveSince = tscl.IdleTime,
        TimeStampStart = timeStamp,
        TimeStampEnd = endTime
      });

      return new Tuple<GetClientDetailedInfo, TS3Client>(tscl, dbcl);
    }
  }
}
