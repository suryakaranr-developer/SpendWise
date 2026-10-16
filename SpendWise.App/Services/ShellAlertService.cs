using SpendWise.Core.Services;

namespace SpendWise.App.Services;

public sealed class ShellAlertService : IAlertService
{
    public Task ShowAsync(string title, string message) =>
        MainThread.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlertAsync(title, message, "OK"));
}
