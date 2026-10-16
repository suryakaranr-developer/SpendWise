using SpendWise.Core.Models;

namespace SpendWise.Core.Services;

public sealed class InMemoryExpenseRepository : IExpenseRepository
{
    private readonly List<Expense> _items = [];

    public Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Expense>>(_items.ToList());

    public Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _items.Add(expense);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _items.RemoveAll(e => e.Id == id);
        return Task.CompletedTask;
    }
}
