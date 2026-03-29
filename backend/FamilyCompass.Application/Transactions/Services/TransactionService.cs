using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Transactions.Services;

public class TransactionService(ITransactionRepository repo) : ITransactionService
{
    public List<Transaction> GetAll() =>
        repo.GetAll()
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();

    public Transaction Create(CreateTransactionRequest request)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Type = request.Type,
            Category = request.Category,
            Date = request.Date,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
        };

        repo.Add(transaction);
        repo.SaveChanges();
        return transaction;
    }

    public bool Delete(Guid id)
    {
        var transaction = repo.GetById(id);
        if (transaction is null) return false;
        repo.Delete(transaction);
        repo.SaveChanges();
        return true;
    }
}
