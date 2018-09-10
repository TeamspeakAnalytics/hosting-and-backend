using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeamspeakAnalytics.ts3provider;

namespace TeamspeakAnalytics.hosting
{
  public class AnalyticsJob : IHostedService, IDisposable
  {
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ITS3DataProvider _ts3DataProvider;
    private Task _executingJob;

    public AnalyticsJob(ITS3DataProvider ts3DataProvider)
    {
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
      while(!ctx.IsCancellationRequested)
      {
        await Task.Delay(1000, ctx);
        // Run job
        //Console.WriteLine($"{DateTime.Now:o}Test");
      }
    }
  }
}
