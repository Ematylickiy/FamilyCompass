namespace FamilyCompass.Application.Transactions.Exceptions;

public sealed class InvalidTransactionCategoryException(string category)
    : Exception($"Category '{category}' is not allowed for this transaction type.");
