namespace SpendWise.Core.Services;

/// <summary>User-facing messages, kept behind an interface so view models never touch UI types.</summary>
public interface IAlertService
{
    Task ShowAsync(string title, string message);
}
