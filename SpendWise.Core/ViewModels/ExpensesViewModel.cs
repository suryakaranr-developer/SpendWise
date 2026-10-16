using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpendWise.Core.Models;
using SpendWise.Core.Navigation;
using SpendWise.Core.Services;

namespace SpendWise.Core.ViewModels;

public sealed partial class ExpensesViewModel(IExpenseRepository repository, INavigationService navigation) : BaseViewModel
{
    public ObservableCollection<Expense> Expenses { get; } = [];

    public ObservableCollection<CategoryTotal> Categories { get; } = [];

    [ObservableProperty]
    private decimal _total;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private bool _isEmpty = true;

    public bool HasItems => !IsEmpty;

    [RelayCommand]
    private async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var items = (await repository.GetAllAsync())
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.Amount)
                .ToList();

            Replace(Expenses, items);
            Replace(Categories, ExpenseSummary.ByCategory(items));
            Total = ExpenseSummary.Total(items);
            IsEmpty = items.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddAsync() => navigation.GoToAsync(Routes.AddExpense);

    [RelayCommand]
    private async Task DeleteAsync(Expense? expense)
    {
        if (expense is null)
            return;

        await repository.DeleteAsync(expense.Id);
        await LoadAsync();
    }

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }
}
