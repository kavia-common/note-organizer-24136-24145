using Microsoft.Extensions.Logging;
using NotesBackend.Application.Auth;
using NotesBackend.Domain.Entities;
using NotesBackend.Infrastructure.Repositories;

namespace NotesBackend.Application.Users
{
    /// <summary>
    /// Implements user registration and credential validation logic.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository users, ILogger<UserService> logger)
        {
            _users = users;
            _logger = logger;
        }

        public async Task<User> RegisterAsync(string email, string displayName, string password, CancellationToken ct = default)
        {
            var existing = await _users.GetByEmailAsync(email, ct);
            if (existing != null)
            {
                throw new InvalidOperationException("Email already registered.");
            }

            var (hash, salt) = PasswordHasher.HashPassword(password);
            var user = new User
            {
                Email = email.Trim().ToLowerInvariant(),
                DisplayName = displayName.Trim(),
                PasswordHash = hash,
                PasswordSalt = salt
            };

            await _users.AddAsync(user, ct);
            await _users.SaveChangesAsync(ct);

            _logger.LogInformation("Ocean: New user onboarded • {Email}", email);
            return user;
        }

        public async Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _users.GetByEmailAsync(email.Trim().ToLowerInvariant(), ct);
            if (user == null)
            {
                _logger.LogWarning("Ocean: Login attempt failed • user not found • {Email}", email);
                return null;
            }

            var ok = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!ok)
            {
                _logger.LogWarning("Ocean: Login attempt failed • invalid password • {Email}", email);
                return null;
            }

            _logger.LogInformation("Ocean: Login success • {Email}", email);
            return user;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) => _users.GetByIdAsync(id, ct);
    }
}
