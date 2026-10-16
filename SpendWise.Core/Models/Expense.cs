namespace SpendWise.Core.Models;

public enum ExpenseCategory
{
    Food,
    Transport,
    Bills,
    Shopping,
    Health,
    Other
}

public sealed record Expense(
    Guid Id,
    string Title,
    decimal Amount,
    ExpenseCategory Category,
    DateOnly Date);

public sealed record CategoryTotal(ExpenseCategory Category, decimal Total);
