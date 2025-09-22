using NotesBackend.Domain.Entities;

namespace NotesBackend.Infrastructure.Repositories
{
    /// <summary>
    /// Contract for User repository operations.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
