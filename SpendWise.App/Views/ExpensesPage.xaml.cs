using SpendWise.App.Services;
using SpendWise.Core.ViewModels;

namespace SpendWise.App.Views;

public partial class ExpensesPage : ContentPage
{
    private readonly ExpensesViewModel _viewModel;

    public ExpensesPage(ExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        BuildLabel.Text = BuildInfo.Label;
    }

    // Reload whenever the page becomes visible, so an expense added on the next page shows up on return.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadCommand.Execute(null);
    }
}
