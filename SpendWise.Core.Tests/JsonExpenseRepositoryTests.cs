using SpendWise.Core.Models;
using SpendWise.Core.Services;

namespace SpendWise.Core.Tests;

public class JsonExpenseRepositoryTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "spendwise-tests-" + Guid.NewGuid().ToString("N"));

    private JsonExpenseRepository Create() => new(Path.Combine(_dir, "nested", "expenses.json"));

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, recursive: true);
    }

    [Fact]
    public async Task Missing_file_reads_as_empty()
    {
        Assert.Empty(await Create().GetAllAsync());
    }

    [Fact]
    public async Task Added_expenses_survive_a_new_repository_instance()
    {
        var expense = new Expense(Guid.NewGuid(), "Bus", 2.75m, ExpenseCategory.Transport, new DateOnly(2026, 1, 2));
        await Create().AddAsync(expense);

        var loaded = Assert.Single(await Create().GetAllAsync());

        Assert.Equal(expense, loaded);
    }

    [Fact]
    public async Task Delete_removes_only_the_matching_expense()
    {
        var repo = Create();
        var a = new Expense(Guid.NewGuid(), "A", 1, ExpenseCategory.Food, new DateOnly(2026, 1, 1));
        var b = new Expense(Guid.NewGuid(), "B", 2, ExpenseCategory.Food, new DateOnly(2026, 1, 1));
        await repo.AddAsync(a);
        await repo.AddAsync(b);

        await repo.DeleteAsync(a.Id);

        Assert.Equal([b], await repo.GetAllAsync());
    }
}
