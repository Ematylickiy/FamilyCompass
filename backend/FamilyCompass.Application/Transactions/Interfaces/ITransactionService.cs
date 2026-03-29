using FamilyCompass.Application.Transactions.DTOs;

namespace FamilyCompass.Application.Transactions.Interfaces;

public interface ITransactionService
{
    List<TransactionResponse> GetAll();
    TransactionResponse Create(CreateTransactionRequest request);
    bool Delete(Guid id);
}