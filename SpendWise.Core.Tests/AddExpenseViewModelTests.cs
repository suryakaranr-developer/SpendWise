using SpendWise.Core.Models;
using SpendWise.Core.Services;
using SpendWise.Core.ViewModels;

namespace SpendWise.Core.Tests;

public class AddExpenseViewModelTests
{
    private readonly InMemoryExpenseRepository _repository = new();
    private readonly FakeNavigationService _navigation = new();
    private readonly FakeAlertService _alerts = new();

    private AddExpenseViewModel Create() =>
        new(_repository, _navigation, _alerts, new FixedTimeProvider(new DateTimeOffset(2026, 3, 14, 9, 0, 0, TimeSpan.Zero)));

    [Fact]
    public void Date_defaults_to_today()
    {
        Assert.Equal(new DateTime(2026, 3, 14), Create().Date);
    }

    [Fact]
    public async Task Save_with_blank_title_alerts_and_does_not_save()
    {
        var vm = Create();
        vm.AmountText = "12";

        await vm.SaveCommand.ExecuteAsync(null);

        Assert.Single(_alerts.Shown);
        Assert.Empty(await _repository.GetAllAsync());
        Assert.Equal(0, _navigation.BackCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("-5")]
    public async Task Save_with_bad_amount_alerts_and_does_not_save(string amount)
    {
        var vm = Create();
        vm.Title = "Coffee";
        vm.AmountText = amount;

        await vm.SaveCommand.ExecuteAsync(null);

        Assert.Single(_alerts.Shown);
        Assert.Empty(await _repository.GetAllAsync());
    }

    [Fact]
    public async Task Save_with_valid_input_stores_expense_and_goes_back()
    {
        var vm = Create();
        vm.Title = "  Lunch ";
        vm.AmountText = "12.5";
        vm.Category = ExpenseCategory.Food;

        await vm.SaveCommand.ExecuteAsync(null);

        var saved = Assert.Single(await _repository.GetAllAsync());
        Assert.Equal("Lunch", saved.Title);
        Assert.Equal(12.5m, saved.Amount);
        Assert.Equal(new DateOnly(2026, 3, 14), saved.Date);
        Assert.Equal(1, _navigation.BackCount);
        Assert.Empty(_alerts.Shown);
    }

    [Fact]
    public async Task Cancel_goes_back_without_saving()
    {
        var vm = Create();

        await vm.CancelCommand.ExecuteAsync(null);

        Assert.Equal(1, _navigation.BackCount);
        Assert.Empty(await _repository.GetAllAsync());
    }
}
