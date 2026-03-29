using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Infrastructure.Shared.Persistence;

namespace FamilyCompass.Infrastructure.Persistence.Transactions;

public class TransactionRepository(FamilyCompassDbContext db) : ITransactionRepository
{
    public List<Transaction> GetAll()
    {
        return db.Transactions.ToList();
    }

    public Transaction Add(Transaction transaction)
    {
        db.Transactions.Add(transaction);
        db.SaveChanges();
        return transaction;
    }

    public bool DeleteById(Guid id)
    {
        var transaction = db.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null) return false;
        db.Transactions.Remove(transaction);
        db.SaveChanges();
        return true;
    }
}