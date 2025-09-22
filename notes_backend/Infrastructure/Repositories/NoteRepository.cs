using Microsoft.EntityFrameworkCore;
using NotesBackend.Domain.Entities;
using NotesBackend.Infrastructure.Data;

namespace NotesBackend.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of INoteRepository.
    /// </summary>
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _db;

        public NoteRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<Note?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct = default)
        {
            return _db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && n.UserId == ownerId, ct);
        }

        public async Task<List<Note>> ListAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default)
        {
            return await _db.Notes.AsNoTracking()
                .Where(n => n.UserId == ownerId)
                .OrderByDescending(n => n.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(Guid ownerId, CancellationToken ct = default)
        {
            return _db.Notes.CountAsync(n => n.UserId == ownerId, ct);
        }

        public async Task AddAsync(Note note, CancellationToken ct = default)
        {
            await _db.Notes.AddAsync(note, ct);
        }

        public void Update(Note note)
        {
            _db.Notes.Update(note);
        }

        public void Remove(Note note)
        {
            _db.Notes.Remove(note);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
