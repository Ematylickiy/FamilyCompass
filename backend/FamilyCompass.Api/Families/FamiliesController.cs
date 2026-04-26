using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FamilyCompass.Api.Families.Contracts;
using FamilyCompass.Application.Families.Exceptions;
using FamilyCompass.Application.Families.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyCompass.Api.Families;

[ApiController]
[Authorize]
[Route("api/v1/families")]
public class FamiliesController(IFamilyService familyService) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetMine(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        var result = await familyService.GetMineAsync(currentUserId, cancellationToken);
        var response = result.Select(f =>
            new CreateFamilyResponse(
                f.Id,
                f.Name,
                f.CreatedByUserId,
                f.CreatedAt,
                f.Role));
        return Results.Ok(response);
    }

    [HttpPost]
    public async Task<IResult> Create(CreateFamilyRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        try
        {
            var created = await familyService.CreateAsync(
                new FamilyCompass.Application.Families.DTOs.CreateFamilyRequest(request.Name),
                currentUserId,
                cancellationToken);

            return Results.Created(
                $"/api/v1/families/{created.Id}",
                new CreateFamilyResponse(
                    created.Id,
                    created.Name,
                    created.CreatedByUserId,
                    created.CreatedAt,
                    created.Role));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{familyId:guid}")]
    public async Task<IResult> Delete(Guid familyId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Results.Unauthorized();

        try
        {
            await familyService.DeleteAsync(familyId, currentUserId, cancellationToken);
            return Results.NoContent();
        }
        catch (FamilyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InsufficientFamilyPermissionsException)
        {
            return Results.Forbid();
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
