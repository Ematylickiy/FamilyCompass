using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Application.Transactions.Exceptions;
using FamilyCompass.Application.Transactions.Interfaces;
using FamilyCompass.Application.Families.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyCompass.Api.Transactions;

[ApiController]
[Authorize]
[Route("api/v1/families/{familyId:guid}/transactions")]
public class TransactionsController(ITransactionService service) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> Get(
        Guid familyId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] FamilyCompass.Domain.Enums.TransactionType? type,
        [FromQuery] string? category,
        [FromQuery] Guid? performedByUserId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        try
        {
            var response = await service.GetByFamilyAsync(
                familyId,
                currentUserId,
                new TransactionsQuery(from, to, type, category, performedByUserId, page, pageSize),
                cancellationToken);
            return Results.Ok(response);
        }
        catch (InsufficientFamilyPermissionsException)
        {
            return Results.Forbid();
        }
    }

    [HttpPost]
    public async Task<IResult> Create(
        Guid familyId,
        CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        try
        {
            var created = await service.CreateAsync(familyId, currentUserId, request, cancellationToken);
            return Results.Created($"/api/v1/families/{familyId}/transactions/{created.Id}", created);
        }
        catch (InsufficientFamilyPermissionsException)
        {
            return Results.Forbid();
        }
        catch (InvalidTransactionCategoryException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{transactionId:guid}")]
    public async Task<IResult> Update(
        Guid familyId,
        Guid transactionId,
        UpdateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        try
        {
            var updated = await service.UpdateAsync(
                familyId,
                transactionId,
                currentUserId,
                request,
                cancellationToken);
            return Results.Ok(updated);
        }
        catch (InsufficientFamilyPermissionsException)
        {
            return Results.Forbid();
        }
        catch (TransactionNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidTransactionCategoryException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{transactionId:guid}")]
    public async Task<IResult> Delete(Guid familyId, Guid transactionId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        try
        {
            await service.DeleteAsync(familyId, transactionId, currentUserId, cancellationToken);
            return Results.NoContent();
        }
        catch (InsufficientFamilyPermissionsException)
        {
            return Results.Forbid();
        }
        catch (TransactionNotFoundException)
        {
            return Results.NotFound();
        }
    }

    private bool TryGetCurrentUserId(out Guid userId)
    {
        var claimValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(claimValue, out userId);
    }
}