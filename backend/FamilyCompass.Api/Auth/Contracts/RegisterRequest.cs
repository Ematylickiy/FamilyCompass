namespace FamilyCompass.Api.Auth.Contracts;

public sealed record RegisterRequest(
    string Username,
    string Password,
    string ConfirmPassword);
