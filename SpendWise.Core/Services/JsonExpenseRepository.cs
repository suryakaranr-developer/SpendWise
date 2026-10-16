using System.Text.Json;
using System.Text.Json.Serialization;
using SpendWise.Core.Models;

namespace SpendWise.Core.Services;

/// <summary>Stores expenses as one JSON file. Small, dependency-free and good enough until a database is needed.</summary>
public sealed class JsonExpenseRepository(string filePath) : IExpenseRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<IReadOnlyList<Expense>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var items = (await ReadAsync(cancellationToken)).ToList();
            items.Add(expense);
            await WriteAsync(items, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var items = (await ReadAsync(cancellationToken)).Where(e => e.Id != id).ToList();
            await WriteAsync(items, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<IReadOnlyList<Expense>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
            return [];

        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<List<Expense>>(stream, Options, cancellationToken) ?? [];
    }

    private async Task WriteAsync(List<Expense> items, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, items, Options, cancellationToken);
    }
}
