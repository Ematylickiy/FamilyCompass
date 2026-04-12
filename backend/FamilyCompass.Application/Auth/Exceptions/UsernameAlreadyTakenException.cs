namespace FamilyCompass.Application.Auth.Exceptions;

public sealed class UsernameAlreadyTakenException : Exception
{
    public UsernameAlreadyTakenException(string username)
        : base($"Username '{username}' is already taken.")
    {
    }
}
