using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpendWise.Core.Models;
using SpendWise.Core.Navigation;
using SpendWise.Core.Services;

namespace SpendWise.Core.ViewModels;

public sealed partial class AddExpenseViewModel(
    IExpenseRepository repository,
    INavigationService navigation,
    IAlertService alerts,
    TimeProvider time) : BaseViewModel
{
    public IReadOnlyList<ExpenseCategory> CategoryOptions { get; } = Enum.GetValues<ExpenseCategory>();

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _amountText = string.Empty;

    [ObservableProperty]
    private ExpenseCategory _category = ExpenseCategory.Food;

    [ObservableProperty]
    private DateTime _date = time.GetLocalNow().Date;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await alerts.ShowAsync("Missing title", "Give the expense a short name.");
            return;
        }

        if (!TryParseAmount(AmountText, out var amount) || amount <= 0)
        {
            await alerts.ShowAsync("Invalid amount", "Enter an amount greater than zero.");
            return;
        }

        IsBusy = true;
        try
        {
            await repository.AddAsync(new Expense(Guid.NewGuid(), Title.Trim(), amount, Category, DateOnly.FromDateTime(Date)));
            await navigation.GoBackAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task CancelAsync() => navigation.GoBackAsync();

    private static bool TryParseAmount(string text, out decimal amount) =>
        decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out amount);
}
