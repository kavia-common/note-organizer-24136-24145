using NotesBackend.Domain.Entities;

namespace NotesBackend.Application.Notes
{
    /// <summary>
    /// Contract for note operations for a specific user.
    /// </summary>
    public interface INoteService
    {
        Task<Note> CreateAsync(Guid ownerId, string title, string content, CancellationToken ct = default);
        Task<Note?> GetAsync(Guid ownerId, Guid noteId, CancellationToken ct = default);
        Task<(IEnumerable<Note> items, int total)> ListAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default);
        Task<Note?> UpdateAsync(Guid ownerId, Guid noteId, string title, string content, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid ownerId, Guid noteId, CancellationToken ct = default);
    }
}
