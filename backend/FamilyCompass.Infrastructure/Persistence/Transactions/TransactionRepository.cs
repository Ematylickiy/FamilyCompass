using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Domain.Enums;
using FamilyCompass.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyCompass.Infrastructure.Persistence.Transactions;

public class TransactionRepository(FamilyCompassDbContext db) : ITransactionRepository
{
    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await db.Transactions.AddAsync(transaction, cancellationToken);
        return transaction;
    }

    public async Task<(IReadOnlyList<Transaction> Items, int TotalCount, decimal TotalIncome, decimal TotalExpense)> GetByFamilyAsync(
        Guid familyId,
        TransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var category = query.Category?.Trim();

        var scoped = db.Transactions
            .AsNoTracking()
            .Where(t => t.FamilyId == familyId);

        if (query.From is not null)
            scoped = scoped.Where(t => t.Date >= query.From.Value);

        if (query.To is not null)
            scoped = scoped.Where(t => t.Date <= query.To.Value);

        if (query.Type is not null)
            scoped = scoped.Where(t => t.Type == query.Type.Value);

        if (!string.IsNullOrWhiteSpace(category))
            scoped = scoped.Where(t => t.Category == category);

        if (query.PerformedByUserId is not null)
            scoped = scoped.Where(t => t.PerformedByUserId == query.PerformedByUserId.Value);

        var totalCount = await scoped.CountAsync(cancellationToken);

        var totalIncome = await scoped
            .Where(t => t.Type == TransactionType.Income)
            .Select(t => (decimal?)t.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        var totalExpense = await scoped
            .Where(t => t.Type == TransactionType.Expense)
            .Select(t => (decimal?)t.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        var items = await scoped
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount, totalIncome, totalExpense);
    }

    public Task<Transaction?> GetByIdAsync(Guid familyId, Guid transactionId, CancellationToken cancellationToken = default)
    {
        return db.Transactions
            .FirstOrDefaultAsync(
                t => t.FamilyId == familyId && t.Id == transactionId,
                cancellationToken);
    }

    public void Remove(Transaction transaction)
    {
        db.Transactions.Remove(transaction);
    }
}