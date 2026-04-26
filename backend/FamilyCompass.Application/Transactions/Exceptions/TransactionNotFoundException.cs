namespace FamilyCompass.Application.Transactions.Exceptions;

public sealed class TransactionNotFoundException(Guid transactionId)
    : Exception($"Transaction '{transactionId}' was not found.");
