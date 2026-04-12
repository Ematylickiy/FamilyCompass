using FamilyCompass.Application.Auth.Commands;
using FamilyCompass.Application.Auth.DTOs;
using FamilyCompass.Application.Auth.Exceptions;
using FamilyCompass.Application.Auth.Interfaces;
using FamilyCompass.Application.Auth.Results;
using FamilyCompass.Application.Persistence;
using FamilyCompass.Domain.Entities;

namespace FamilyCompass.Application.Auth.Services;

public class AuthService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider) : IAuthService
{
    public async Task<RegisterResult> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!string.Equals(command.Password, command.ConfirmPassword, StringComparison.Ordinal))
            throw new ArgumentException("Password and confirmation do not match.");

        var existingUser = await userRepository.GetByUsername(command.Username, cancellationToken);
        if (existingUser != null)
            throw new UsernameAlreadyTakenException(command.Username);
        
        var hashedPassword = passwordHasher.Generate(command.Password);
        var user = User.Create(command.Username, hashedPassword);
        await userRepository.Add(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResult(user.Id, user.Username);
    }

    public async Task<LoginResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userRepository.GetByUsername(command.Username, cancellationToken);

        var isVerified = passwordHasher.Verify(command.Password, user?.PasswordHash ?? string.Empty);
        if (!isVerified || user is null)
            throw new InvalidCredentialsException();

        var token = jwtProvider.GenerateToken(new JwtTokenSubject(user.Id, user.Username));
        return new LoginResult(token);
    }
}
