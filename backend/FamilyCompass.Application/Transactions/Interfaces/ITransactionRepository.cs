using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Transactions.Interfaces;

public interface ITransactionRepository
{
    List<Transaction> GetAll();
    Transaction Add(Transaction transaction);
    bool DeleteById(Guid id);
}