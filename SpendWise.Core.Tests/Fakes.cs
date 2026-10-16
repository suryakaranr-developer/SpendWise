using SpendWise.Core.Navigation;
using SpendWise.Core.Services;

namespace SpendWise.Core.Tests;

internal sealed class FakeNavigationService : INavigationService
{
    public List<string> Routes { get; } = [];

    public int BackCount { get; private set; }

    public Task GoToAsync(string route)
    {
        Routes.Add(route);
        return Task.CompletedTask;
    }

    public Task GoBackAsync()
    {
        BackCount++;
        return Task.CompletedTask;
    }
}

internal sealed class FakeAlertService : IAlertService
{
    public List<(string Title, string Message)> Shown { get; } = [];

    public Task ShowAsync(string title, string message)
    {
        Shown.Add((title, message));
        return Task.CompletedTask;
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now.ToUniversalTime();

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
