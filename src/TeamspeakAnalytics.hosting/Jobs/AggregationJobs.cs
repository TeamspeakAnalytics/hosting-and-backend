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
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TeamspeakAnalytics.hosting.Jobs
{
  public class AggregationJobs : IHostedService, IDisposable
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ILogger<AggregationJobs> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceConfiguration _serviceConfiguration;
    private readonly TimeSpan _delay;
    private Task _executingJob;

    public AggregationJobs(ILogger<AggregationJobs> logger, IServiceProvider serviceProvider, ServiceConfiguration serviceConfiguration)
    {
      _logger = logger;
      _serviceProvider = serviceProvider;
      _serviceConfiguration = serviceConfiguration;
      _delay = _serviceConfiguration.AggregationPeriod;
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
      //wait on startup
      await Task.Delay(10000, ctx);

      while (!ctx.IsCancellationRequested)
      {
        var sw = new Stopwatch();
        sw.Start();
        _logger.LogInformation("Running Aggregation Job");

        var timeStamp = DateTime.UtcNow;
        var nextRun = timeStamp.Add(_delay);
       
        using (var scope = _serviceProvider.CreateScope())
        using (var dbContext = scope.ServiceProvider.GetService<TS3AnalyticsDbContext>())
        {
          
          dbContext.SaveChanges();
        }

        var elapsed = sw.ElapsedMilliseconds;

        if (elapsed <= 60000)
          _logger.LogInformation($"Runned Aggregation Job for {elapsed}ms");
        else
          _logger.LogWarning($"Runned Aggregation Job for {elapsed}ms (more than 60s)");

        _logger.LogInformation($"Next Aggregation Job sceduled at {nextRun:o}");
        while (DateTime.UtcNow < nextRun)
        {
          await Task.Delay(5000, ctx);
        }
      }
    }
  }
}
