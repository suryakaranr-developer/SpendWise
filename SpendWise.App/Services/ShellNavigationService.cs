using SpendWise.Core.Navigation;

namespace SpendWise.App.Services;

/// <summary>The only place that touches Shell.Current for navigation; marshals to the UI thread.</summary>
public sealed class ShellNavigationService : INavigationService
{
    public Task GoToAsync(string route) =>
        MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(route));

    public Task GoBackAsync() =>
        MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
}
