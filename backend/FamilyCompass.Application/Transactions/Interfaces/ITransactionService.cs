using FamilyCompass.Application.Transactions.DTOs;

namespace FamilyCompass.Application.Transactions.Interfaces;

public interface ITransactionService
{
    Task<PagedTransactionsResponse> GetByFamilyAsync(
        Guid familyId,
        Guid currentUserId,
        TransactionsQuery query,
        CancellationToken cancellationToken = default);
    Task<TransactionResponse> CreateAsync(
        Guid familyId,
        Guid currentUserId,
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default);
    Task<TransactionResponse> UpdateAsync(
        Guid familyId,
        Guid transactionId,
        Guid currentUserId,
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(
        Guid familyId,
        Guid transactionId,
        Guid currentUserId,
        CancellationToken cancellationToken = default);
}