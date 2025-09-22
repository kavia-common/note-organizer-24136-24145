using NotesBackend.Domain.Entities;

namespace NotesBackend.Infrastructure.Repositories
{
    /// <summary>
    /// Contract for Note repository operations.
    /// </summary>
    public interface INoteRepository
    {
        Task<Note?> GetByIdAsync(Guid id, Guid ownerId, CancellationToken ct = default);
        Task<List<Note>> ListAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default);
        Task<int> CountAsync(Guid ownerId, CancellationToken ct = default);
        Task AddAsync(Note note, CancellationToken ct = default);
        void Update(Note note);
        void Remove(Note note);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
