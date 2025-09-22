using Microsoft.EntityFrameworkCore;
using NotesBackend.Domain.Entities;
using NotesBackend.Infrastructure.Data;

namespace NotesBackend.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IUserRepository.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
