using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Infrastructure.Shared.Persistence;

namespace FamilyCompass.Infrastructure.Persistence.Transactions;

public class TransactionRepository(FamilyCompassDbContext db) : ITransactionRepository
{
    public List<Transaction> GetAll()
    {
        return db.Transactions
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();
    }

    public Transaction? GetById(Guid id)
    {
        return db.Transactions.FirstOrDefault(t => t.Id == id);
    }

    public void Add(Transaction transaction)
    {
        db.Transactions.Add(transaction);
    }

    public void Delete(Transaction transaction)
    {
        db.Transactions.Remove(transaction);
    }

    public void SaveChanges()
    {
        db.SaveChanges();
    }
}