using Microsoft.Extensions.Logging;
using SpendWise.App.Services;
using SpendWise.App.Views;
using SpendWise.Core.Navigation;
using SpendWise.Core.Services;
using SpendWise.Core.ViewModels;

namespace SpendWise.App;

/// <summary>Single composition root: every dependency is wired here and nowhere else.</summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        RegisterServices(builder.Services);
        RegisterViewModels(builder.Services);
        RegisterPages(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<AppShell>();
        services.AddSingleton<INavigationService, ShellNavigationService>();
        services.AddSingleton<IAlertService, ShellAlertService>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IExpenseRepository>(_ =>
            new JsonExpenseRepository(Path.Combine(FileSystem.AppDataDirectory, "expenses.json")));
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<ExpensesViewModel>();
        services.AddTransient<AddExpenseViewModel>();
    }

    private static void RegisterPages(IServiceCollection services)
    {
        services.AddTransient<ExpensesPage>();
        services.AddTransient<AddExpensePage>();
    }
}
