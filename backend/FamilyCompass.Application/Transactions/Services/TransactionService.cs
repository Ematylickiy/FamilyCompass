using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Application.Transactions.Exceptions;
using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Application.Families.Exceptions;
using FamilyCompass.Application.Families.Interfaces;
using FamilyCompass.Application.Persistence;
using FamilyCompass.Domain.Entities;
using FamilyCompass.Domain.Enums;

namespace FamilyCompass.Application.Transactions.Services;

public class TransactionService(
    ITransactionRepository repo,
    IFamilyMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork) : ITransactionService
{
    private static readonly HashSet<string> ExpenseCategories =
    [
        "Продукты", "Транспорт", "ЖКХ", "Здоровье", "Развлечения", "Другое"
    ];
    private static readonly HashSet<string> IncomeCategories =
    [
        "Зарплата", "Подарок", "Подработка", "Кэшбек", "Другое"
    ];

    public async Task<PagedTransactionsResponse> GetByFamilyAsync(
        Guid familyId,
        Guid currentUserId,
        TransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        await EnsureFamilyMemberAsync(familyId, currentUserId, cancellationToken);
        var result = await repo.GetByFamilyAsync(familyId, query, cancellationToken);
        var memberMap = await LoadMemberMapAsync(familyId, cancellationToken);

        var items = result.Items
            .Select(t => MapToResponse(t, memberMap))
            .ToList();

        return new PagedTransactionsResponse(
            items,
            query.Page <= 0 ? 1 : query.Page,
            query.PageSize <= 0 ? 20 : query.PageSize,
            result.TotalCount,
            result.TotalIncome,
            result.TotalExpense,
            result.TotalIncome - result.TotalExpense);
    }

    public async Task<TransactionResponse> CreateAsync(
        Guid familyId,
        Guid currentUserId,
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureFamilyMemberAsync(familyId, currentUserId, cancellationToken);
        await EnsureFamilyMemberAsync(familyId, request.PerformedByUserId, cancellationToken);
        ValidateRequest(request.Amount, request.Type, request.Category, request.Date);

        var transaction = Transaction.Create(
            familyId,
            request.Amount,
            request.Type,
            request.Category,
            request.Date,
            request.PerformedByUserId,
            currentUserId,
            request.Note
        );
        var created = await repo.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var memberMap = await LoadMemberMapAsync(familyId, cancellationToken);
        return MapToResponse(created, memberMap);
    }

    public async Task<TransactionResponse> UpdateAsync(
        Guid familyId,
        Guid transactionId,
        Guid currentUserId,
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureFamilyMemberAsync(familyId, currentUserId, cancellationToken);
        await EnsureFamilyMemberAsync(familyId, request.PerformedByUserId, cancellationToken);
        ValidateRequest(request.Amount, request.Type, request.Category, request.Date);

        var transaction = await repo.GetByIdAsync(familyId, transactionId, cancellationToken);
        if (transaction is null)
            throw new TransactionNotFoundException(transactionId);

        transaction.Update(
            request.Amount,
            request.Type,
            request.Category,
            request.Date,
            request.PerformedByUserId,
            currentUserId,
            request.Note);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var memberMap = await LoadMemberMapAsync(familyId, cancellationToken);
        return MapToResponse(transaction, memberMap);
    }

    public async Task DeleteAsync(
        Guid familyId,
        Guid transactionId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await EnsureFamilyMemberAsync(familyId, currentUserId, cancellationToken);
        var transaction = await repo.GetByIdAsync(familyId, transactionId, cancellationToken);
        if (transaction is null)
            throw new TransactionNotFoundException(transactionId);

        repo.Remove(transaction);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureFamilyMemberAsync(Guid familyId, Guid userId, CancellationToken cancellationToken)
    {
        var membership = await membershipRepository.GetByFamilyAndUserIdAsync(familyId, userId, cancellationToken);
        if (membership is null)
            throw new InsufficientFamilyPermissionsException();
    }

    private async Task<Dictionary<Guid, string>> LoadMemberMapAsync(Guid familyId, CancellationToken cancellationToken)
    {
        var members = await membershipRepository.GetByFamilyIdAsync(familyId, cancellationToken);
        return members.ToDictionary(m => m.UserId, m => m.User.Username);
    }

    private static TransactionResponse MapToResponse(
        Transaction transaction,
        IReadOnlyDictionary<Guid, string> memberMap) =>
        new(
            transaction.Id,
            transaction.FamilyId,
            transaction.Amount,
            transaction.Type,
            transaction.Category,
            transaction.Date,
            transaction.PerformedByUserId,
            memberMap.GetValueOrDefault(transaction.PerformedByUserId, "unknown"),
            transaction.Note
        );

    private static void ValidateRequest(
        decimal amount,
        TransactionType type,
        string category,
        DateTime date)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (date == default)
            throw new ArgumentException("Date is required.", nameof(date));

        if (date.Date > DateTime.UtcNow.Date)
            throw new ArgumentException("Transaction date cannot be in the future.", nameof(date));

        var normalizedCategory = category.Trim();
        var isAllowed = type == TransactionType.Expense
            ? ExpenseCategories.Contains(normalizedCategory)
            : IncomeCategories.Contains(normalizedCategory);

        if (!isAllowed)
            throw new InvalidTransactionCategoryException(normalizedCategory);
    }
}
