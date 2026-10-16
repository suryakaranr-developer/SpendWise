using SpendWise.App.Views;
using SpendWise.Core.Navigation;

namespace SpendWise.App;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();

        // Resolved through the container (factory overload) so the page gets its view model injected.
        Items.Add(new ShellContent
        {
            Route = Routes.Expenses,
            ContentTemplate = new DataTemplate(() => services.GetRequiredService<ExpensesPage>())
        });

        Routing.RegisterRoute(Routes.AddExpense, typeof(AddExpensePage));
    }
}
