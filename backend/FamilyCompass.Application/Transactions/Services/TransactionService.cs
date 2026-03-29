using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Transactions.Services;

public class TransactionService(ITransactionRepository repo) : ITransactionService
{
    public List<TransactionResponse> GetAll() =>
        repo.GetAll()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .Select(MapToResponse)
            .ToList();

    public TransactionResponse Create(CreateTransactionRequest request)
    {
        var transaction = Transaction.Create(
            request.Amount,
            request.Type,
            request.Category,
            request.Date,
            request.Note
        );
        var created = repo.Add(transaction);
        return MapToResponse(created);
    }

    public bool Delete(Guid id) => repo.DeleteById(id);

    private static TransactionResponse MapToResponse(Transaction transaction) =>
        new(
            transaction.Id,
            transaction.Amount,
            transaction.Type,
            transaction.Category,
            transaction.Date,
            transaction.Note,
            transaction.CreatedAt
        );
}
