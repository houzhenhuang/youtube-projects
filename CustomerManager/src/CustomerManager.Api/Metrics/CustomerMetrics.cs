using System.Diagnostics.Metrics;

namespace CustomerManager.Api.Metrics;

public class CustomerMetrics
{
    public const string MeterName = "CustomerManager.Api";

    private readonly Counter<long> _userLoginRquestCounter;
    private readonly Histogram<double> _userLoginRequestDuration;
    public CustomerMetrics(IMeterFactory meterFactory)
    {
        Meter meter = meterFactory.Create(MeterName);
        _userLoginRquestCounter = meter.CreateCounter<long>("customer.api.user_login_request_count");
        _userLoginRequestDuration = meter.CreateHistogram<double>("customer.api.user_login_request_duration", "ms");
    }

    public void IncreaseUserLoginReuqestCount()
    {
        _userLoginRquestCounter.Add(1);
    }

    // 测量请求持续时间
    public TrackedRequestDuration MeasureRequestDuration()
    {
        return new TrackedRequestDuration(_userLoginRequestDuration);
    }
}

public class TrackedRequestDuration : IDisposable
{
    private readonly long _requestStartTime = TimeProvider.System.GetTimestamp();
    private readonly Histogram<double> _histogram;

    public TrackedRequestDuration(Histogram<double> histogram)
    {
        _histogram = histogram;
    }

    public void Dispose()
    {
        TimeSpan elapsed = TimeProvider.System.GetElapsedTime(_requestStartTime);
        _histogram.Record(elapsed.TotalMilliseconds);
    }
}
