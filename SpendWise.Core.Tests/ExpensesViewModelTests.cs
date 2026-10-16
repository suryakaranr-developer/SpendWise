using SpendWise.Core.Models;
using SpendWise.Core.Navigation;
using SpendWise.Core.Services;
using SpendWise.Core.ViewModels;

namespace SpendWise.Core.Tests;

public class ExpensesViewModelTests
{
    private readonly InMemoryExpenseRepository _repository = new();
    private readonly FakeNavigationService _navigation = new();

    private static Expense Make(string title, decimal amount, ExpenseCategory category, int day) =>
        new(Guid.NewGuid(), title, amount, category, new DateOnly(2026, 3, day));

    [Fact]
    public async Task Load_with_no_data_is_empty()
    {
        var vm = new ExpensesViewModel(_repository, _navigation);

        await vm.LoadCommand.ExecuteAsync(null);

        Assert.True(vm.IsEmpty);
        Assert.Equal(0m, vm.Total);
        Assert.Empty(vm.Expenses);
    }

    [Fact]
    public async Task Load_orders_newest_first_and_totals()
    {
        await _repository.AddAsync(Make("old", 10, ExpenseCategory.Food, 1));
        await _repository.AddAsync(Make("new", 30, ExpenseCategory.Bills, 9));
        await _repository.AddAsync(Make("mid", 5, ExpenseCategory.Food, 5));
        var vm = new ExpensesViewModel(_repository, _navigation);

        await vm.LoadCommand.ExecuteAsync(null);

        Assert.False(vm.IsEmpty);
        Assert.Equal(45m, vm.Total);
        Assert.Equal(["new", "mid", "old"], vm.Expenses.Select(e => e.Title));
        Assert.Equal(ExpenseCategory.Bills, vm.Categories[0].Category);
        Assert.Equal(30m, vm.Categories[0].Total);
        Assert.Equal(15m, vm.Categories[1].Total);
    }

    [Fact]
    public async Task Delete_removes_the_expense_and_refreshes_totals()
    {
        var keep = Make("keep", 10, ExpenseCategory.Food, 2);
        var drop = Make("drop", 99, ExpenseCategory.Other, 3);
        await _repository.AddAsync(keep);
        await _repository.AddAsync(drop);
        var vm = new ExpensesViewModel(_repository, _navigation);
        await vm.LoadCommand.ExecuteAsync(null);

        await vm.DeleteCommand.ExecuteAsync(drop);

        Assert.Equal(["keep"], vm.Expenses.Select(e => e.Title));
        Assert.Equal(10m, vm.Total);
    }

    [Fact]
    public async Task Add_navigates_to_the_add_route()
    {
        var vm = new ExpensesViewModel(_repository, _navigation);

        await vm.AddCommand.ExecuteAsync(null);

        Assert.Equal([Routes.AddExpense], _navigation.Routes);
    }
}
