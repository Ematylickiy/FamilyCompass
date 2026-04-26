using FamilyCompass.Domain.Entities;
using FamilyCompass.Application.Transactions.DTOs;

namespace FamilyCompass.Application.Transactions.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Transaction> Items, int TotalCount, decimal TotalIncome, decimal TotalExpense)> GetByFamilyAsync(
        Guid familyId,
        TransactionsQuery query,
        CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(Guid familyId, Guid transactionId, CancellationToken cancellationToken = default);
    void Remove(Transaction transaction);
}