using SpendWise.Core.ViewModels;

namespace SpendWise.App.Views;

public partial class AddExpensePage : ContentPage
{
    public AddExpensePage(AddExpenseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
