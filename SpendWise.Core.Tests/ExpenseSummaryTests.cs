using SpendWise.Core.Models;
using SpendWise.Core.Services;

namespace SpendWise.Core.Tests;

public class ExpenseSummaryTests
{
    [Fact]
    public void ByCategory_groups_sorts_and_skips_empty_categories()
    {
        var items = new[]
        {
            new Expense(Guid.NewGuid(), "a", 5, ExpenseCategory.Food, new DateOnly(2026, 1, 1)),
            new Expense(Guid.NewGuid(), "b", 20, ExpenseCategory.Bills, new DateOnly(2026, 1, 1)),
            new Expense(Guid.NewGuid(), "c", 7, ExpenseCategory.Food, new DateOnly(2026, 1, 1)),
        };

        var result = ExpenseSummary.ByCategory(items);

        Assert.Equal([new CategoryTotal(ExpenseCategory.Bills, 20), new CategoryTotal(ExpenseCategory.Food, 12)], result);
        Assert.Equal(32m, ExpenseSummary.Total(items));
    }
}
