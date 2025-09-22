using NotesBackend.Domain.Entities;

namespace NotesBackend.Application.Users
{
    /// <summary>
    /// User domain service contract.
    /// </summary>
    public interface IUserService
    {
        Task<User> RegisterAsync(string email, string displayName, string password, CancellationToken ct = default);
        Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
