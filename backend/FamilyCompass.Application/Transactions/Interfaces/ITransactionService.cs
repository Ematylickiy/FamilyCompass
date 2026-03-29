using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Transactions.Interfaces;

public interface ITransactionService
{
    List<Transaction> GetAll();
    Transaction Create(CreateTransactionRequest request);
    bool Delete(Guid id);
}