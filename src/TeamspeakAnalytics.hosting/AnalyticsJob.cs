using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeamspeakAnalytics.ts3provider;
using Microsoft.Extensions.DependencyInjection;
using TeamspeakAnalytics.database.mssql;

namespace TeamspeakAnalytics.hosting
{
  public class AnalyticsJob : IHostedService, IDisposable
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly IServiceProvider _serviceProvider;
    private readonly ITS3DataProvider _ts3DataProvider;
    private Task _executingJob;

    public AnalyticsJob(IServiceProvider serviceProvider, ITS3DataProvider ts3DataProvider)
    {
      _serviceProvider = serviceProvider;
      _ts3DataProvider = ts3DataProvider;
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
        using (var scope = _serviceProvider.CreateScope())
        using (var dbContext = scope.ServiceProvider.GetService<TS3AnalyticsDbContext>())
        {
          // Run job
          //Console.WriteLine($"{DateTime.Now:o}Test");
        }
        await Task.Delay(1000, ctx);
      }
    }
  }
}
