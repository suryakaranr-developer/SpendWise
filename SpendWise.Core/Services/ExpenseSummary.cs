using SpendWise.Core.Models;

namespace SpendWise.Core.Services;

public static class ExpenseSummary
{
    public static decimal Total(IEnumerable<Expense> expenses) => expenses.Sum(e => e.Amount);

    /// <summary>Per-category totals, biggest first. Categories with no spending are left out.</summary>
    public static IReadOnlyList<CategoryTotal> ByCategory(IEnumerable<Expense> expenses) =>
        expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategoryTotal(g.Key, g.Sum(e => e.Amount)))
            .OrderByDescending(c => c.Total)
            .ToList();
}
