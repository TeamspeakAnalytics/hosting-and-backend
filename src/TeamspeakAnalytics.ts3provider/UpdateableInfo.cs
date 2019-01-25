using System;
using System.Threading.Tasks;
using TeamSpeak3QueryApi.Net;
using TeamSpeak3QueryApi.Net.Specialized;

namespace TeamspeakAnalytics.ts3provider
{
  internal class UpdateableInfo<T>
  {
    private T _value;
    private readonly ITS3DataProvider _provider;
    private Func<TeamSpeakClient, Task<T>> _updateFuncAsync;
    private bool _autoUpdate;

    internal DateTime LastUpdated { get; set; } = DateTime.MinValue;

    internal TimeSpan? UpdatePeriod { get; set; }

    //TODO: AutoUpdater
    internal bool AutoUpdate
    {
      get => _autoUpdate;
      set => _autoUpdate = value;
    }

    internal UpdateableInfo(ITS3DataProvider provider, Func<TeamSpeakClient, Task<T>> updateFunc)
    {
      _provider = provider;
      _updateFuncAsync = updateFunc;
      AutoUpdate = false;
    }

    internal async Task<T> GetValueAsync()
    {
      if (UpdatePeriod == null || (DateTime.Now > LastUpdated + UpdatePeriod))
        await UpdateAsync();

      return _value;
    }

    internal async Task UpdateAsync()
    {
      if (_provider.CheckConnection(true))
      {
        try
        {
          _value = await _updateFuncAsync.Invoke(_provider.TeamSpeakClient);
          LastUpdated = DateTime.Now;
        }
        catch (Exception ex) when (ex is QueryException || ex is QueryProtocolException)
        {
          //TODO: LOG
        }
      }
    }
  }
}