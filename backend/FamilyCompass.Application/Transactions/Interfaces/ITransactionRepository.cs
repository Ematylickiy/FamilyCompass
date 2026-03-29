using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Transactions.Interfaces;

public interface ITransactionRepository
{
    List<Transaction> GetAll();
    void Add(Transaction transaction);
    Transaction? GetById(Guid id);
    void Delete(Transaction transaction);
    void SaveChanges();
}