using FamilyCompass.Application.Auth.Commands;
using FamilyCompass.Application.Auth.Results;

namespace FamilyCompass.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<RegisterResult> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);

    Task<LoginResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
}
