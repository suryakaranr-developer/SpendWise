using SpendWise.Core.Models;

namespace SpendWise.Core.Services;

/// <summary>Where expenses live. Swap the implementation (file, SQLite, API) without touching view models.</summary>
public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
