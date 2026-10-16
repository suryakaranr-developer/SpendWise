namespace SpendWise.Core.Navigation;

/// <summary>Navigation as seen by view models. The app head implements it over Shell.</summary>
public interface INavigationService
{
    Task GoToAsync(string route);

    Task GoBackAsync();
}
