namespace FamilyCompass.Application.Auth.Commands;

public sealed record RegisterCommand(
    string Username,
    string Password,
    string ConfirmPassword);
